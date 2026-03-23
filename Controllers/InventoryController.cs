using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models.db;
using Microsoft.EntityFrameworkCore;

namespace SirinEngineering.Controllers
{
    public class InventoryController : Controller
    {
        private readonly projectContext _db;
        private readonly IWebHostEnvironment _hostEnvironment;
        public InventoryController(projectContext db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult ManageProduct(string? search, int page = 1)
        {
            int pageSize = 6;

            var query = _db.TBL_Product
                           .Include(p => p.Category)
                           .AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(p => p.PD_ProductName.Contains(search));
                ViewBag.Search = search;
            }

            // Pagination
            int totalItems = query.Count();

            var products = query
                .OrderByDescending(p => p.PD_ProductID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);
            ViewBag.Categories = _db.TBL_Category.ToList();

            return View(products);
        }

        [HttpPost]
        public async Task<IActionResult> CreateProduct(ProductModel model, IFormFile? ImageFile)
        {
            if (ImageFile != null)
            {
                // บันทึกรูปลง wwwroot/images/products
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string path = Path.Combine(wwwRootPath, "images/products");

                if (!Directory.Exists(path)) Directory.CreateDirectory(path);

                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                model.PD_ProductImage = fileName;
            }

            _db.TBL_Product.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction("ManageProduct");
        }
        [HttpPost]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _db.TBL_Product.FindAsync(id);

            if (product == null)
                return NotFound();

            if (!string.IsNullOrEmpty(product.PD_ProductImage))
            {
                var path = Path.Combine(_hostEnvironment.WebRootPath, "images/products", product.PD_ProductImage);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }
            }

            _db.TBL_Product.Remove(product);
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageProduct");
        }

        public IActionResult ManageCategory(string? search, int page = 1)
        {
            int pageSize = 6;

            var query = _db.TBL_Category.AsQueryable();

            // Search
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(c => c.C_CategoryName.Contains(search));
                ViewBag.Search = search;
            }

            // Pagination
            int totalItems = query.Count();

            var categories = query
                .OrderByDescending(c => c.C_CategoryID)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new CategoryModel
                {
                    C_CategoryID = c.C_CategoryID,
                    C_CategoryName = c.C_CategoryName,
                    C_ProductCount = _db.TBL_Product.Count(p => p.PD_CategoryID == c.C_CategoryID)
                })
                .ToList();

            ViewBag.CurrentPage = page;
            ViewBag.TotalPages = (int)Math.Ceiling((double)totalItems / pageSize);

            return View(categories);
        }

        [HttpPost]
        public IActionResult CreateCategory(CategoryModel model)
        {
            if (model != null)
            {
                _db.TBL_Category.Add(model);
                _db.SaveChanges();
            }
            return RedirectToAction("ManageCategory");
        }

        [HttpPost]
        public IActionResult EditCategory(CategoryModel model)
        {
            var category = _db.TBL_Category.Find(model.C_CategoryID);

            if (category == null)
                return NotFound();

            category.C_CategoryName = model.C_CategoryName;

            _db.TBL_Category.Update(category);
            _db.SaveChanges();

            return RedirectToAction("ManageCategory");
        }

        [HttpPost]
        [HttpPost]
        public IActionResult DeleteCategory(int id)
        {
            var category = _db.TBL_Category.Find(id);

            if (category == null)
                return NotFound();

            // 🔥 เช็คว่ามี Product ใช้อยู่ไหม
            bool hasProduct = _db.TBL_Product.Any(p => p.PD_CategoryID == id);

            if (hasProduct)
            {
                TempData["Error"] = "ไม่สามารถลบหมวดหมู่นี้ได้ เนื่องจากมีสินค้าอยู่";
                return RedirectToAction("ManageCategory");
            }

            _db.TBL_Category.Remove(category);
            _db.SaveChanges();

            TempData["Success"] = "ลบหมวดหมู่เรียบร้อยแล้ว";
            return RedirectToAction("ManageCategory");
        }

        [HttpPost]
        public async Task<IActionResult> SaveProduct(ProductModel model, IFormFile? ImageFile)
        {
            // ตรวจสอบเบื้องต้น
            if (model.PD_Price < 0 || model.PD_Cost < 0) return BadRequest("ราคาห้ามติดลบ");

            var existingProduct = await _db.TBL_Product.FindAsync(model.PD_ProductID);

            if (existingProduct != null)
            {
                // --- กรณี "แก้ไข" หรือ "เติมสต็อก" ---
                existingProduct.PD_ProductName = model.PD_ProductName;
                existingProduct.PD_Brand = model.PD_Brand;
                existingProduct.PD_Price = model.PD_Price;
                existingProduct.PD_Cost = model.PD_Cost;
                existingProduct.PD_CategoryID = model.PD_CategoryID;
                existingProduct.PD_IsActive = model.PD_IsActive;

                // Logic: เอาจำนวนที่กรอกมา "บวกเพิ่ม" เข้าไปในสต็อกเดิม
                existingProduct.PD_StockQty += model.PD_StockQty;

                if (ImageFile != null)
                {
                    existingProduct.PD_ProductImage = await UploadImage(ImageFile);
                }
            }
            else
            {
                // --- กรณี "เพิ่มใหม่" ---
                if (ImageFile != null) model.PD_ProductImage = await UploadImage(ImageFile);
                _db.TBL_Product.Add(model);
            }

            await _db.SaveChangesAsync();
            return RedirectToAction("ManageProduct");
        }

        // Function สำหรับอัปโหลดรูป
        private async Task<string> UploadImage(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            string path = Path.Combine(_hostEnvironment.WebRootPath, "images/products", fileName);
            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return fileName;
        }

        [HttpPost]
        public async Task<IActionResult> EditProduct(ProductModel model, IFormFile? ImageFile)
        {
            // 1. ดึงข้อมูลสินค้าเดิมจากฐานข้อมูล
            var existingProduct = await _db.TBL_Product.FindAsync(model.PD_ProductID);

            if (existingProduct == null)
            {
                return NotFound();
            }

            // 2. ตรวจสอบเงื่อนไขราคาและต้นทุนห้ามติดลบ (Server-side Validation)
            if (model.PD_Price < 0 || model.PD_Cost < 0 || model.PD_StockQty < 0)
            {
                ModelState.AddModelError("", "ราคา ต้นทุน หรือจำนวนสต็อก ห้ามมีค่าติดลบ");
                // ถ้าผิดพลาด ให้ดึงข้อมูลกลับไปแสดงที่หน้าเดิม
                return RedirectToAction("ManageProduct");
            }

            // 3. อัปเดตข้อมูล
            existingProduct.PD_ProductName = model.PD_ProductName;
            existingProduct.PD_Brand = model.PD_Brand;
            existingProduct.PD_CategoryID = model.PD_CategoryID;
            existingProduct.PD_Price = model.PD_Price;
            existingProduct.PD_Cost = model.PD_Cost;
            existingProduct.PD_MinStock = model.PD_MinStock;
            existingProduct.PD_IsActive = model.PD_IsActive;

            // 4. บวกเพิ่มจากของเดิมที่มีอยู่
            existingProduct.PD_StockQty += model.PD_StockQty;

            // 5. การจัดการรูปภาพ (ถ้ามีการอัปโหลดใหม่)
            if (ImageFile != null)
            {
                string wwwRootPath = _hostEnvironment.WebRootPath;
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(ImageFile.FileName);
                string path = Path.Combine(wwwRootPath, "images/products");

                // ลบรูปเก่าออกก่อนเพื่อประหยัดพื้นที่
                if (!string.IsNullOrEmpty(existingProduct.PD_ProductImage))
                {
                    var oldPath = Path.Combine(path, existingProduct.PD_ProductImage);
                    if (System.IO.File.Exists(oldPath)) System.IO.File.Exists(oldPath);
                }

                using (var fileStream = new FileStream(Path.Combine(path, fileName), FileMode.Create))
                {
                    await ImageFile.CopyToAsync(fileStream);
                }
                existingProduct.PD_ProductImage = fileName;
            }

            // 6. บันทึกการเปลี่ยนแปลง
            _db.TBL_Product.Update(existingProduct);
            await _db.SaveChangesAsync();

            return RedirectToAction("ManageProduct");
        }

        public IActionResult ManagePromotion()
        {
            // แสดงหน้าจัดการโปรโมชัน (ยังไม่ทำฟังก์ชัน)
            return View();
        }
    }
}