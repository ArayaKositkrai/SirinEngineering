using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models.db;
using SirinEngineering.ViewModels;
using System.Linq;
using System.Security.Claims; // เพิ่ม
using Microsoft.AspNetCore.Authentication; // เพิ่ม
using System.Threading.Tasks; // เพิ่ม
using System.Collections.Generic;

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
        public async Task<IActionResult> Login(AccountViewModel model)
        {
            var user = _db.TBL_User.FirstOrDefault(u =>
                u.U_Username == model.Login.Username &&
                u.U_Password == model.Login.Password &&
                u.U_IsActive);

            if (user != null)
            {
                // แปลง RoleID เป็นชื่อ Role
                string roleName = user.U_RoleID == 1 ? "Admin" : (user.U_RoleID == 2 ? "Staff" : "Customer");

                // --- สร้างบัตรประจำตัว (Cookie) ให้ระบบจำ User ---
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.U_Username), // เก็บชื่อ Username
                    new Claim(ClaimTypes.Role, roleName)         // เก็บสิทธิ์ (Admin, Staff, Customer)
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));

                // --- แยกเส้นทาง ---
                switch (user.U_RoleID)
                {
                    case 1: return RedirectToAction("Dashboard", "Admin");
                    case 2: return RedirectToAction("StockCheck", "Sales");
                    case 3: return RedirectToAction("Index", "Product");
                    default: return RedirectToAction("Login", "Account");
                }
            }

            ModelState.AddModelError(string.Empty, "ชื่อผู้ใช้หรือรหัสผ่านไม่ถูกต้อง");
            model.ActiveTab = "login";
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Register(AccountViewModel model)
        {
            if (string.IsNullOrEmpty(model.Register.Username) || string.IsNullOrEmpty(model.Register.Password))
            {
                ModelState.AddModelError(string.Empty, "กรุณากรอกข้อมูลให้ครบถ้วน");
            }
            else if (_db.TBL_User.Any(u => u.U_Username == model.Register.Username))
            {
                ModelState.AddModelError(string.Empty, "Username นี้ถูกใช้ไปแล้ว");
            }
            else
            {
                var newUser = new UserModel
                {
                    U_Username = model.Register.Username,
                    U_Password = model.Register.Password,
                    U_FullName = model.Register.FullName,
                    U_Phone = model.Register.Tel,
                    U_RoleID = 3, // Customer
                    U_IsActive = true
                };
                _db.TBL_User.Add(newUser);
                _db.SaveChanges();

                // สมัครเสร็จแล้วให้ Login อัตโนมัติเลย
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, newUser.U_Username),
                    new Claim(ClaimTypes.Role, "Customer")
                };
                var identity = new ClaimsIdentity(claims, "MyCookieAuth");
                await HttpContext.SignInAsync("MyCookieAuth", new ClaimsPrincipal(identity));

                return RedirectToAction("Index", "Product");
            }

            model.ActiveTab = "register";
            return View("Login", model);
        }

        // --- เพิ่มฟังก์ชัน Logout ---
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("MyCookieAuth");
            return RedirectToAction("Index", "Product");
        }
    }
}