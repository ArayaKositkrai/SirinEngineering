using Microsoft.AspNetCore.Mvc;

namespace SirinEngineering.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult StaffManage()
        {
            // แสดงหน้าจัดการพนักงาน (ยังไม่ทำฟังก์ชัน)
            return View();
        }

        public IActionResult AuditOrders()
        {
            // แสดงหน้าจัดการออเดอร์ (ยังไม่ทำฟังก์ชัน)
            return View();
        }

        public IActionResult ManageProduct()
        {
            // แสดงหน้าจัดการสินค้า (ยังไม่ทำฟังก์ชัน)
            return View();
        }

        public IActionResult ManagePromotion()
        {
            // แสดงหน้าจัดการโปรโมชั่น (ยังไม่ทำฟังก์ชัน)
            return View();
        }
    }
}