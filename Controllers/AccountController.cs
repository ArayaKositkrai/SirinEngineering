using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models.db;
using SirinEngineering.ViewModels;
using System.Linq;

namespace SirinEngineering.Controllers
{
    public class AccountController : Controller
    {
        private readonly projectContext _db;

        public AccountController(projectContext db)
        {
            _db = db;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new AccountViewModel());
        }

        [HttpPost]
        public IActionResult Login(AccountViewModel model)
        {
            // 1. ตรวจสอบ Username และ Password จากฐานข้อมูล
            var user = _db.TBL_User.FirstOrDefault(u =>
                u.U_Username == model.Login.Username &&
                u.U_Password == model.Login.Password &&
                u.U_IsActive);

            if (user != null)
            {
                // 2. แยกเส้นทางตาม RoleID (1=Admin, 2=Staff, 3=Customer)
                switch (user.U_RoleID)
                {
                    case 1: // Admin -> ไปหน้า Dashboard
                        return RedirectToAction("Dashboard", "Admin");

                    case 2: // Staff -> ไปหน้าเช็คสต็อก
                        return RedirectToAction("StockCheck", "Sales");

                    case 3: // Customer -> ไปหน้าดูรายการสินค้า
                        return RedirectToAction("Index", "Product");

                    default: // กรณีอื่นๆ ให้กลับมาหน้า Login
                        return RedirectToAction("Login", "Account");
                }
            }

            // 3. ถ้า Login ไม่สำเร็จ ให้แสดง Error และค้างไว้ที่ Tab Login
            ModelState.AddModelError(string.Empty, "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
            model.ActiveTab = "login";
            return View(model);
        }

        [HttpPost]
        public IActionResult Register(AccountViewModel model)
        {
            // ตรวจสอบเฉพาะข้อมูลในส่วน Register เท่านั้น
            if (string.IsNullOrEmpty(model.Register.Username) || string.IsNullOrEmpty(model.Register.Password))
            {
                ModelState.AddModelError(string.Empty, "กรุณากรอกข้อมูลให้ครบถ้วน");
            }
            else
            {
                // ตรวจสอบว่า Username ซ้ำไหม
                if (_db.TBL_User.Any(u => u.U_Username == model.Register.Username))
                {
                    ModelState.AddModelError(string.Empty, "Username นี้ถูกใช้ไปแล้ว");
                }
                else
                {
                    // สร้าง User ใหม่
                    var newUser = new UserModel
                    {
                        U_Username = model.Register.Username,
                        U_Password = model.Register.Password,
                        U_FullName = model.Register.FullName,
                        U_Phone = model.Register.Tel,
                        U_RoleID = 3, // 3 = Customer
                        U_IsActive = true
                    };

                    _db.TBL_User.Add(newUser);
                    _db.SaveChanges();

                    // สมัครเสร็จให้ไปหน้า Login และแจ้งว่าสำเร็จ (ถ้าต้องการ)
                    return RedirectToAction("Index", "Product");
                }
            }

            // ถ้ามี Error ให้ค้างไว้ที่หน้า Register
            model.ActiveTab = "register";
            return View("Login", model);
        }
    }
}