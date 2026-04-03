using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models.db;
using SirinEngineering.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace SirinEngineering.Controllers;

public class ProductController : Controller
{
    private readonly projectContext _db;

    public ProductController(projectContext db)
    {
        _db = db;
    }

    public IActionResult Index()
    {
        ViewBag.Categories = _db.TBL_Category.ToList();
        var featuredProducts = _db.TBL_Product.Take(20).ToList(); // ดึงมา 8 ชิ้นเพื่อให้เรียงได้ 2 แถว
        return View(featuredProducts);
    }

    public IActionResult ProductList(int? categoryId, string searchKeyword)
    {
        ViewBag.Categories = _db.TBL_Category.ToList();
        var products = _db.TBL_Product.AsQueryable();

        // 1. กรองตามคำค้นหา (ชื่อสินค้า หรือ ยี่ห้อ)
        if (!string.IsNullOrEmpty(searchKeyword))
        {
            products = products.Where(p => p.PD_ProductName.Contains(searchKeyword) || p.PD_Brand.Contains(searchKeyword));
            ViewBag.SearchKeyword = searchKeyword; // ส่งคำกลับไปโชว์ในช่องค้นหา
        }

        // 2. กรองตามหมวดหมู่ (ทำร่วมกับการค้นหาได้)
        if (categoryId.HasValue && categoryId > 0)
        {
            products = products.Where(p => p.PD_CategoryID == categoryId);
            ViewBag.CurrentCategory = categoryId;
        }

        return View(products.ToList());
    }

    [HttpGet]
    public IActionResult Cart()
    {
        var cart = GetCartFromSession();

        // ดึงโปรโมชั่นที่ยัง Active อยู่ และอยู่ในช่วงเวลาที่กำหนด
        ViewBag.Promotions = _db.TBL_Promotion
                           .Where(p => p.PM_IsActive == true && p.PM_EndDate >= DateTime.Now)
                           .ToList();

        // ดึงค่าการจัดส่งและโปรโมชั่นที่เคยเลือกไว้ (ถ้ามี)
        string shippingMethod = HttpContext.Session.GetString("ShippingMethod") ?? "Delivery";
        string promoCode = HttpContext.Session.GetString("PromoCode") ?? "";

        // --- การคำนวณยอดเงิน (ทำด้วย C# ล้วนๆ) ---
        decimal subTotal = 0;
        foreach (var item in cart.Where(c => c.IsSelected))
        {
            subTotal += (item.Price * item.Quantity);
        }

        decimal shippingFee = 0;
        if (shippingMethod == "Delivery")
        {
            shippingFee = (subTotal >= 2000 || subTotal == 0) ? 0 : 150;
        }

        decimal promoDiscount = (promoCode == "SIRIN100") ? 100 : 0;
        decimal grandTotal = (subTotal + shippingFee) - promoDiscount;
        if (grandTotal < 0) grandTotal = 0;

        // ส่งค่าไปที่ View
        ViewBag.ShippingMethod = shippingMethod;
        ViewBag.PromoCode = promoCode;
        ViewBag.SubTotal = subTotal;
        ViewBag.ShippingFee = shippingFee;
        ViewBag.PromoDiscount = promoDiscount;
        ViewBag.GrandTotal = grandTotal;

        return View(cart);
    }

    [HttpPost]
    public IActionResult UpdateCart(List<int> ProductIds, List<int> Quantities, List<int> SelectedProducts, string ShippingMethod, string PromoCode, string ActionType)
    {
        var cart = GetCartFromSession();

        // 1. อัปเดตจำนวน และสถานะการเลือก (ติ๊กถูก)
        for (int i = 0; i < cart.Count; i++)
        {
            // จับคู่ ID สินค้า เพื่ออัปเดตจำนวนให้ถูกต้อง
            int indexInForm = ProductIds.IndexOf(cart[i].ProductId);
            if (indexInForm >= 0)
            {
                cart[i].Quantity = Quantities[indexInForm];
                // เช็คว่า ID นี้ถูกส่งมาจาก Checkbox ไหม
                cart[i].IsSelected = SelectedProducts.Contains(cart[i].ProductId);
            }
        }

        // 2. บันทึกตะกร้า, ค่าจัดส่ง และโปรโมชั่น ลง Session
        SaveCartToSession(cart);
        HttpContext.Session.SetString("ShippingMethod", ShippingMethod ?? "Delivery");
        HttpContext.Session.SetString("PromoCode", PromoCode ?? "");

        // 3. เช็คว่าลูกค้ากดปุ่มไหนมา ("Update" หรือ "Checkout")
        if (ActionType == "Checkout")
        {
            // ถ้ากดสั่งซื้อ ให้ดึงเฉพาะของที่เลือกลง Session ใหม่ แล้วพาไปหน้า Checkout
            var checkoutItems = cart.Where(c => c.IsSelected).ToList();
            HttpContext.Session.SetString("CheckoutItems", JsonConvert.SerializeObject(checkoutItems));

            return RedirectToAction("Checkout");
        }

        // ถ้ากด "อัปเดตตะกร้า" ให้กลับไปหน้า Cart เพื่อโหลดตัวเลขใหม่
        return RedirectToAction("Cart");
    }

    [HttpPost]
    public IActionResult AddToCart(int productId, int qty)
    {
        // --- จุดที่ 1: เช็คว่า Login หรือยัง ---
        if (!User.Identity.IsAuthenticated)
        {
            // ถ้ายังไม่ Login ให้เด้งไปหน้า Login ของ AccountController
            return RedirectToAction("Login", "Account");
        }

        // --- จุดที่ 2: ดึงข้อมูลสินค้าจาก DB ---
        var product = _db.TBL_Product.FirstOrDefault(p => p.PD_ProductID == productId);
        if (product == null) return NotFound();

        // --- จุดที่ 3: จัดการของในตะกร้า (Session) ---
        var cart = GetCartFromSession();
        var item = cart.FirstOrDefault(x => x.ProductId == productId);

        if (item != null)
        {
            item.Quantity += qty; // ถ้ามีอยู่แล้วให้บวกเพิ่ม
        }
        else
        {
            cart.Add(new CartItemViewModel
            {
                CategoryId = product.PD_CategoryID,
                ProductId = product.PD_ProductID,
                ProductName = product.PD_ProductName,
                Price = product.PD_Price,
                Image = product.PD_ProductImage,
                Quantity = qty
            });
        }

        SaveCartToSession(cart); // บันทึกลง Session

        return RedirectToAction("Cart");
    }

    // 3. ฟังก์ชันลบสินค้า
    public IActionResult RemoveFromCart(int id)
    {
        var cart = GetCartFromSession();
        cart.RemoveAll(x => x.ProductId == id);
        SaveCartToSession(cart);
        return RedirectToAction("Cart");
    }

    // --- Helper Methods สำหรับจัดการ Session ---
    private List<CartItemViewModel> GetCartFromSession()
    {
        var sessionData = HttpContext.Session.GetString("Cart");
        return sessionData == null ? new List<CartItemViewModel>() : JsonConvert.DeserializeObject<List<CartItemViewModel>>(sessionData);
    }

    private void SaveCartToSession(List<CartItemViewModel> cart)
    {
        HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
    }

    [HttpGet]
    public IActionResult MyOrders()
    {
        var currentUsername = User.Identity.Name;
        var user = _db.TBL_User.FirstOrDefault(u => u.U_Username == currentUsername);
        if (user == null) return RedirectToAction("Login", "Account");

        // ดึงข้อมูลจาก OrderModel ของจริง
        var myOrders = _db.TBL_Order
                          .Where(o => o.O_UserID == user.U_UserID)
                          .OrderByDescending(o => o.O_OrderDate)
                          .ToList();

        return View(myOrders);
    }

    // 1. รับค่าจากตะกร้าสินค้า (เตรียมข้อมูลส่งไปหน้า Checkout)
    [HttpPost]
    public IActionResult ProceedToCheckout(List<int> SelectedProducts, List<int> Quantities, List<int> ProductIds, string ShippingMethod)
    {
        var currentUsername = User.Identity.Name;
        var user = _db.TBL_User.FirstOrDefault(u => u.U_Username == currentUsername);
        if (user == null) return RedirectToAction("Login", "Account");

        var cartJson = HttpContext.Session.GetString("Cart");
        var cart = string.IsNullOrEmpty(cartJson) ? new List<CartItemViewModel>() : JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartJson);
        var checkoutItems = new List<CartItemViewModel>();

        for (int i = 0; i < cart.Count; i++)
        {
            if (SelectedProducts != null && SelectedProducts.Contains(cart[i].ProductId))
            {
                int indexInForm = ProductIds.IndexOf(cart[i].ProductId);
                if (indexInForm >= 0) cart[i].Quantity = Quantities[indexInForm];
                checkoutItems.Add(cart[i]);
            }
        }

        // 🌟 เก็บรายการสินค้าและรูปแบบการส่ง (Pickup/Delivery) ลง Session
        HttpContext.Session.SetString("CheckoutItems", JsonConvert.SerializeObject(checkoutItems));
        HttpContext.Session.SetString("ShippingMethod", ShippingMethod ?? "Delivery");

        // 🌟 ไม่ต้องบันทึก DB ตรงนี้แล้ว! บังคับไปหน้า Checkout เสมอ
        return RedirectToAction("Checkout");
    }

    // 2. หน้า Checkout
    [HttpGet]
    public IActionResult Checkout()
    {
        var currentUsername = User.Identity.Name;
        var user = _db.TBL_User.FirstOrDefault(u => u.U_Username == currentUsername);
        var checkoutJson = HttpContext.Session.GetString("CheckoutItems");
        if (string.IsNullOrEmpty(checkoutJson)) return RedirectToAction("Cart");

        ViewBag.CheckoutItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(checkoutJson);
        return View(user);
    }

    // 3. ยืนยันจากหน้า Checkout -> บันทึก DB -> หักสต๊อก -> ไป MyOrders
    [HttpPost]
    public IActionResult ConfirmOrder(string U_Address)
    {
        var currentUsername = User.Identity.Name;
        var user = _db.TBL_User.FirstOrDefault(u => u.U_Username == currentUsername);
        var checkoutJson = HttpContext.Session.GetString("CheckoutItems");
        
        // 🌟 ดึงรูปแบบการส่งจาก Session มาเช็ค
        var shippingMethod = HttpContext.Session.GetString("ShippingMethod") ?? "Delivery"; 

        if (string.IsNullOrEmpty(checkoutJson)) return RedirectToAction("Cart");

        var checkoutItems = JsonConvert.DeserializeObject<List<CartItemViewModel>>(checkoutJson);

        // 🌟 อัปเดตที่อยู่เฉพาะตอนเลือก "จัดส่ง"
        if (user != null && shippingMethod == "Delivery")
        {
            user.U_Address = U_Address;
            _db.SaveChanges();
        }

        var orderDate = DateTime.Now; // ล็อกเวลาบิล

        foreach (var item in checkoutItems)
        {
            var newOrder = new OrderModel
            {
                O_OrderDate = orderDate, 
                O_CustomerName = user.U_FullName ?? "", 
                O_ProductID = item.ProductId,
                O_ProductName = item.ProductName ?? "", 
                O_Quantity = item.Quantity,
                O_Price = item.Price,
                O_SubTotal = item.Price * item.Quantity,
                O_TotalAmount = item.Price * item.Quantity,
                O_PaymentType = shippingMethod, // 🌟 ใช้ค่าจาก Session (Pickup หรือ Delivery)
                O_UserID = user.U_UserID,
                O_Status = "รอตรวจสอบ",
                O_GiftItemName = "" 
            };
            _db.TBL_Order.Add(newOrder);

            // ตัดสต๊อกสินค้า
            var product = _db.TBL_Product.FirstOrDefault(p => p.PD_ProductID == item.ProductId);
            if (product != null)
            {
                product.PD_StockQty -= item.Quantity;
                if (product.PD_StockQty < 0) product.PD_StockQty = 0; 
                _db.TBL_Product.Update(product);
            }
        }
        _db.SaveChanges(); 

        // ล้างตะกร้า
        var cartJsonOriginal = HttpContext.Session.GetString("Cart");
        if (!string.IsNullOrEmpty(cartJsonOriginal))
        {
            var cart = JsonConvert.DeserializeObject<List<CartItemViewModel>>(cartJsonOriginal);
            var purchasedIds = checkoutItems.Select(x => x.ProductId).ToList();
            cart.RemoveAll(c => purchasedIds.Contains(c.ProductId));
            HttpContext.Session.SetString("Cart", JsonConvert.SerializeObject(cart));
        }
        
        // เคลียร์ Session 
        HttpContext.Session.Remove("CheckoutItems");
        HttpContext.Session.Remove("ShippingMethod");

        return RedirectToAction("MyOrders");
    }
}