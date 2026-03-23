using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models.db;

namespace SirinEngineering.Controllers
{
    public class AdminController : Controller
    {
        private readonly projectContext _db;

        // Constructor เรียกใช้ฐานข้อมูล
        public AdminController(projectContext db)
        {
            _db = db;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        // public IActionResult StaffManage()
        // {
        //     var staffList = _db.TBL_User.Where(u => u.U_RoleID == 1 || u.U_RoleID == 2).ToList();
        //     return View(staffList);
        // }
        // ดึงข้อมูลมาโชว์
        [HttpGet]
        public IActionResult EditStaff(int id)
        {
            var user = _db.TBL_User.Find(id);
            if (user == null) return NotFound();
            return View(user);
        }

        // รับค่าจากฟอร์มแก้ไขไปบันทึก
        [HttpPost]
        public IActionResult EditStaff(UserModel model)
        {
            var user = _db.TBL_User.Find(model.U_UserID);
            if (user != null)
            {
                user.U_FullName = model.U_FullName;
                user.U_Username = model.U_Username;
                user.U_Phone = model.U_Phone;
                user.U_RoleID = model.U_RoleID;
                // เปลี่ยนรหัสผ่านเฉพาะตอนพิมพ์มาใหม่เท่านั้น
                if (!string.IsNullOrEmpty(model.U_Password))
                {
                    user.U_Password = model.U_Password;
                }
                _db.SaveChanges();
            }
            return RedirectToAction("StaffManage");
        }
        [HttpPost]
        public IActionResult CreateStaff(UserModel user)
        {
            if (user != null)
            {
                user.U_IsActive = true;
                _db.TBL_User.Add(user);
                _db.SaveChanges();
            }
            return RedirectToAction("StaffManage");
        }
        public IActionResult DeleteStaff(int id)
        {
            var user = _db.TBL_User.Find(id);
            if (user != null)
            {
                _db.TBL_User.Remove(user);
                _db.SaveChanges();
            }
            return RedirectToAction("StaffManage");
        }

        [HttpPost]
        public IActionResult ToggleUserStatus(int id)
        {
            var user = _db.TBL_User.Find(id);

            if (user == null)
                return NotFound();

            user.U_IsActive = !user.U_IsActive;

            _db.SaveChanges();

            return RedirectToAction("StaffManage");
        }

        public IActionResult StaffManage(string? search)
        {
            var query = _db.TBL_User
                .Where(u => u.U_RoleID == 1 || u.U_RoleID == 2)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(u =>
                    u.U_FullName.Contains(search) ||
                    u.U_Username.Contains(search));

                ViewBag.Search = search;
            }

            var staffList = query.ToList();

            return View(staffList);
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