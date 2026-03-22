using Microsoft.AspNetCore.Mvc;

namespace SirinEngineering.Controllers
{
    public class InventoryController : Controller
    {
        public IActionResult ManageProduct()
        {
            return View();
        }
        public IActionResult ManagePromotion()
        {
            // แสดงหน้าจัดการโปรโมชัน (ยังไม่ทำฟังก์ชัน)
            return View();
        }
    }
}