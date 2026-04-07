using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SirinEngineering.Models.db;
using System.Linq;
using System.Collections.Generic;
using System;

namespace SirinEngineering.Controllers
{
    public class SalesController : Controller
    {
        private readonly projectContext _db;

        // ดึง Database Context มาใช้งาน
        public SalesController(projectContext db)
        {
            _db = db;
        }

        //  1. ฟังก์ชันเปิดหน้า POS (Shop) ที่หายไป เติมกลับมาให้แล้วครับ!
        [HttpGet]
        public IActionResult Shop()
        {
            var products = _db.TBL_Product.Include(p => p.Category).Where(p => p.PD_IsActive && p.PD_StockQty > 0).ToList();
            
            ViewBag.Categories = products.Where(p => p.Category != null)
                                         .Select(p => p.Category.C_CategoryName)
                                         .Distinct()
                                         .OrderBy(c => c)
                                         .ToList();

            ViewBag.Promotions = _db.TBL_Promotion.Where(p => p.PM_IsActive).ToList();

            return View(products);
        }

        // --- คลาสตัวช่วยรับค่า (อัปเดตเพิ่มส่วนลดและของแถม) ---
        public class POSCheckoutRequest {
            public List<POSCartItem> Items { get; set; }
            public string CustomerName { get; set; }
            public decimal DiscountAmount { get; set; } //  รับค่าส่วนลด
            public string GiftItemName { get; set; }    //  รับค่าของแถม
        }
        public class POSCartItem {
            public int ProductId { get; set; }
            public int Qty { get; set; }
        }

        // 2. ฟังก์ชันรับข้อมูลชำระเงิน
        [HttpPost]
        public IActionResult POSCheckout([FromBody] POSCheckoutRequest request)
        {
            if (request == null || request.Items == null || !request.Items.Any())
                return Json(new { success = false, message = "ไม่มีสินค้าในตะกร้า" });

            using (var transaction = _db.Database.BeginTransaction())
            {
                try
                {
                    int nextId = (_db.TBL_Order.Max(o => (int?)o.O_OrderID) ?? 0) + 1;
                    DateTime now = DateTime.Now;

                    string sellerName = User.Identity.IsAuthenticated ? User.Identity.Name : "พนักงานหน้าร้าน";
                    string cusName = string.IsNullOrEmpty(request.CustomerName) ? "ลูกค้าหน้าร้าน (Walk-in)" : request.CustomerName;
                    
                    // เช็คของแถม ถ้าไม่กรอกให้เป็น "-"
                    string giftName = string.IsNullOrWhiteSpace(request.GiftItemName) ? "-" : request.GiftItemName;

                    // คำนวณยอดรวมก่อนลด เพื่อเอาไปหารเฉลี่ยส่วนลดให้สินค้าแต่ละชิ้น (ยอดในบิลจะได้ตรงเป๊ะ)
                    decimal totalOrderValue = 0;
                    foreach(var i in request.Items) {
                        var p = _db.TBL_Product.Find(i.ProductId);
                        if(p != null) totalOrderValue += p.PD_Price * i.Qty;
                    }

                    var savedItems = new List<OrderModel>();

                    foreach (var item in request.Items)
                    {
                        var product = _db.TBL_Product.Find(item.ProductId);
                        if (product != null && product.PD_StockQty >= item.Qty)
                        {
                            product.PD_StockQty -= item.Qty; // ตัดสต๊อก

                            decimal subTotal = product.PD_Price * item.Qty;
                            
                            //  คำนวณส่วนลดเฉลี่ยตามสัดส่วนราคาสินค้า
                            decimal lineDiscount = totalOrderValue > 0 ? (subTotal / totalOrderValue) * request.DiscountAmount : 0;
                            decimal netTotal = subTotal - lineDiscount;

                            var orderItem = new OrderModel
                            {
                                O_OrderID = nextId++, 
                                O_ProductID = product.PD_ProductID,
                                O_ProductName = product.PD_ProductName,
                                O_Quantity = item.Qty,
                                O_Price = product.PD_Price,           
                                O_SubTotal = subTotal,                
                                O_DiscountAmount = lineDiscount,      
                                O_TotalAmount = netTotal,             
                                O_OrderDate = now, 
                                O_CustomerName = cusName,
                                
                                //  แก้ไข 2 บรรทัดนี้สำหรับลูกค้าหน้าร้าน 
                                O_PaymentType = "Walk-in", 
                                O_Status = "ชำระแล้ว",
                                
                                O_SellerName = sellerName,
                                O_GiftItemName = giftName             
                            };
                            
                            _db.TBL_Order.Add(orderItem);
                            savedItems.Add(orderItem);
                        }
                        else
                        {
                            throw new Exception($"สินค้า {product?.PD_ProductName} มีสต๊อกไม่พอ!");
                        }
                    }

                    _db.SaveChanges();
                    transaction.Commit();
                    
                    int displayOrderId = savedItems.First().O_OrderID;
                    return Json(new { success = true, orderId = displayOrderId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return Json(new { success = false, message = ex.InnerException?.Message ?? ex.Message });
                }
            }
        }

        //  ฟังก์ชันเช็คโค้ดส่วนลด (อิงจาก PromotionModel ของจริง)
        [HttpGet]
        public IActionResult CheckPromotion(string code, decimal cartTotal)
        {
            if (string.IsNullOrEmpty(code)) 
                return Json(new { success = false, message = "กรุณากรอกโค้ดส่วนลด" });

            // 🔍 ค้นหาจากชื่อ PM_PromotionName (เพราะใน Model ไม่มีคอลัมน์ Code)
            var promo = _db.TBL_Promotion.FirstOrDefault(p => p.PM_PromotionName == code && p.PM_IsActive);

            if (promo != null)
            {
                DateTime now = DateTime.Now;

                // 1. ตรวจสอบวันเวลา (หมดอายุหรือยัง?)
                if (now < promo.PM_StartDate || now > promo.PM_EndDate)
                {
                    return Json(new { success = false, message = "โปรโมชั่นนี้หมดอายุ หรือยังไม่ถึงเวลาใช้งาน" });
                }

                // 2. ตรวจสอบยอดสั่งซื้อขั้นต่ำ
                if (promo.PM_MinSpend.HasValue && cartTotal < promo.PM_MinSpend.Value)
                {
                    return Json(new { success = false, message = $"ต้องมียอดซื้อขั้นต่ำ ฿{promo.PM_MinSpend.Value:N0}" });
                }

                // 3. คำนวณส่วนลดตามประเภทโปรโมชั่น (PM_PromoType)
                decimal finalDiscount = 0;
                string typeLower = promo.PM_PromoType?.ToLower() ?? "";

                if (typeLower.Contains("percent")) 
                {
                    // ถ้าเป็นเปอร์เซ็นต์ ให้เอา (ยอดรวม x เปอร์เซ็นต์) / 100
                    finalDiscount = cartTotal * ((promo.PM_DiscountValue ?? 0) / 100m);
                } 
                else 
                {
                    // ถ้าเป็นส่วนลดปกติ (Fixed amount)
                    finalDiscount = promo.PM_DiscountValue ?? 0;
                }

                return Json(new { 
                    success = true, 
                    discount = finalDiscount, 
                    name = promo.PM_PromotionName 
                });
            }
            
            return Json(new { success = false, message = "โค้ดส่วนลดไม่ถูกต้อง" });
        }

        public IActionResult StockCheck()
        {
            var products = _db.TBL_Product.Include(p => p.Category).OrderByDescending(p => p.PD_ProductID).ToList();
            return View(products);
        }

        // 1. ฟังก์ชันเปิดหน้าจัดการคำสั่งซื้อ
        [HttpGet]
        public IActionResult OrderManage()
        {
            // ดึงออร์เดอร์ทั้งหมด เรียงจากใหม่ไปเก่า
            var orders = _db.TBL_Order.OrderByDescending(o => o.O_OrderDate).ToList();
            
            // ดึงข้อมูล User เผื่อเอาไปดึงที่อยู่จัดส่งโชว์ใน Modal
            ViewBag.Users = _db.TBL_User.ToList(); 

            return View(orders);
        }

        // 2. ฟังก์ชันอัปเดตสถานะ (เมื่อ Staff กดบันทึกจาก Modal)
        [HttpPost]
        public IActionResult UpdateOrderStatus(List<int> orderIds, string newStatus)
        {
            if (orderIds != null && orderIds.Any())
            {
                // ค้นหาสินค้าทุกชิ้นที่อยู่ใน Order เดียวกัน (อิงจาก ID ที่ส่งมา)
                var ordersToUpdate = _db.TBL_Order.Where(o => orderIds.Contains(o.O_OrderID)).ToList();
                
                foreach (var order in ordersToUpdate)
                {
                    order.O_Status = newStatus; // เปลี่ยนสถานะ
                }
                
                _db.SaveChanges(); // เซฟลงฐานข้อมูล
            }

            return RedirectToAction("OrderManage");
        }
    }
}