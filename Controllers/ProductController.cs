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
        // กรองสินค้าตามหมวดหมู่ถ้ามีการส่งค่ามา
        var products = _db.TBL_Product.AsQueryable();
        if (categoryId.HasValue)
        {
            products = products.Where(p => p.PD_CategoryID == categoryId);
        }
        ViewBag.Categories = _db.TBL_Category.ToList();
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