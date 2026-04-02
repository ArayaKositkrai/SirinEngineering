using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SirinEngineering.Models.db;
using System.Linq;
using System.Collections.Generic;

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

        public IActionResult Shop()
        {
            return View();
        }

        public IActionResult StockCheck()
        {
            return View();
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