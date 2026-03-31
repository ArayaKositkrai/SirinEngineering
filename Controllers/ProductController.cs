using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models.db;
using SirinEngineering.ViewModels;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

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

    [HttpPost]
    public IActionResult AddToCart(int productId, int qty)
    {
        return RedirectToAction("Cart");
    }

    public IActionResult ProductList(int? categoryId)
{
    // 1. ดึงหมวดหมู่ทั้งหมดไปแสดงที่ Sidebar ด้านซ้าย
    ViewBag.Categories = _db.TBL_Category.ToList();

    // 2. ดึงสินค้าทั้งหมดเตรียมไว้
    var products = _db.TBL_Product.AsQueryable();

    // 3. เช็คว่าลูกค้ากดเลือกหมวดหมู่มาหรือไม่ (มีค่า categoryId ส่งมาไหม)
    if (categoryId.HasValue && categoryId > 0)
    {
        // ถ้ามี ให้กรองเอาเฉพาะสินค้าที่ตรงกับหมวดหมู่นั้น
        products = products.Where(p => p.PD_CategoryID == categoryId);
        
        // เก็บค่าหมวดหมู่ปัจจุบันไว้ เพื่อไปทำไฮไลท์สีในหน้าเว็บ
        ViewBag.CurrentCategory = categoryId;
    }

    // 4. ส่งข้อมูลสินค้าที่กรองแล้ว (หรือทั้งหมด) ไปที่หน้า View
    return View(products.ToList());
}

    public IActionResult Cart()
    {
        return View();
    }

    public IActionResult MyOrders()
    {
        return View();
    }
}