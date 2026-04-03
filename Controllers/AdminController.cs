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

        [HttpGet]
        public IActionResult Dashboard(DateTime? exactDate, int? month, int? year)
        {
            var orders = _db.TBL_Order.AsQueryable();
            var today = DateTime.Today;

            // 🌟 1. ลอจิกการกรองตามที่คุณต้องการเป๊ะๆ
            if (exactDate.HasValue)
            {
                // ถ้าเลือกแบบเจาะจงวัน ให้ดูแค่วันนั้น
                orders = orders.Where(o => o.O_OrderDate.Date == exactDate.Value.Date);
            }
            else if (month.HasValue && year.HasValue)
            {
                // ถ้าเลือกทั้งเดือนและปี
                orders = orders.Where(o => o.O_OrderDate.Month == month.Value && o.O_OrderDate.Year == year.Value);
            }
            else if (year.HasValue)
            {
                // ถ้าเลือกแค่ปี ให้โชว์ทั้งปีนั้น
                orders = orders.Where(o => o.O_OrderDate.Year == year.Value);
            }
            else if (month.HasValue)
            {
                // ถ้าเลือกแค่เดือน ให้ดึงเดือนนั้นของ "ปีปัจจุบัน"
                orders = orders.Where(o => o.O_OrderDate.Month == month.Value && o.O_OrderDate.Year == today.Year);
            }
            else
            {
                // Default (เปิดมาตอนแรก): เป็นวันนี้
                orders = orders.Where(o => o.O_OrderDate.Date == today);
            }

            // 2. คำนวณรายได้ทั้งหมด
            decimal totalRevenue = orders.Sum(o => (decimal?)o.O_TotalAmount) ?? 0;

            // 3. คำนวณจำนวนออร์เดอร์
            int orderCount = orders.GroupBy(o => o.O_OrderDate).Count();

            // 4. คำนวณต้นทุนและกำไร 
            var orderWithProducts = from o in orders
                                    join p in _db.TBL_Product on o.O_ProductID equals p.PD_ProductID
                                    select new { o.O_Quantity, o.O_TotalAmount, p.PD_Cost };

            decimal totalCost = orderWithProducts.Sum(x => (decimal?)(x.O_Quantity * x.PD_Cost)) ?? 0;
            decimal totalProfit = totalRevenue - totalCost;

            // 5. สินค้าขายดี Top 5
            var topSelling = (from o in orders
                              join p in _db.TBL_Product on o.O_ProductID equals p.PD_ProductID
                              group new { o, p } by new { p.PD_ProductID, p.PD_ProductName, p.Category.C_CategoryName, p.PD_Cost } into g
                              select new
                              {
                                  Name = g.Key.PD_ProductName,
                                  Category = g.Key.C_CategoryName ?? "-",
                                  UnitCost = g.Key.PD_Cost,
                                  Qty = g.Sum(x => x.o.O_Quantity),
                                  TotalCost = g.Sum(x => x.o.O_Quantity * g.Key.PD_Cost),
                                  TotalSales = g.Sum(x => x.o.O_TotalAmount),
                                  TotalProfit = g.Sum(x => x.o.O_TotalAmount) - g.Sum(x => x.o.O_Quantity * g.Key.PD_Cost)
                              })
                              .OrderByDescending(x => x.Qty)
                              .Take(5)
                              .ToList();

            // 🌟 ส่งค่ากลับไปเพื่อให้ Dropdown คงค่าเดิมที่ถูกเลือกไว้
            ViewBag.ExactDate = exactDate?.ToString("yyyy-MM-dd");
            ViewBag.Month = month;
            ViewBag.Year = year;

            ViewBag.Revenue = totalRevenue;
            ViewBag.OrderCount = orderCount;
            ViewBag.Cost = totalCost;
            ViewBag.Profit = totalProfit;
            ViewBag.TopSelling = topSelling;

            return View();
        }

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

        [HttpGet]
        public IActionResult AuditOrders(string searchKeyword)
        {
            // ดึงออร์เดอร์ทั้งหมดและจัดกลุ่มตามเวลา (เพื่อให้เป็น 1 บิล)
            var groupedOrders = _db.TBL_Order
                .AsEnumerable()
                .GroupBy(o => o.O_OrderDate)
                .OrderByDescending(g => g.Key)
                .ToList();

            // ลอจิกค้นหา
            if (!string.IsNullOrEmpty(searchKeyword))
            {
                var cleanSearch = searchKeyword.Replace("#ORD-", "").Replace("ORD-", "").Replace("#", "").Trim();
                groupedOrders = groupedOrders.Where(g =>
                    g.First().O_OrderID.ToString() == cleanSearch ||
                    (g.First().O_CustomerName != null && g.First().O_CustomerName.Contains(searchKeyword))
                ).ToList();

                ViewBag.SearchKeyword = searchKeyword;
            }
            ViewBag.Users = _db.TBL_User.ToList();

            return View(groupedOrders);
        }
    }
}