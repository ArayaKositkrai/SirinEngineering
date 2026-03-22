using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using SirinEngineering.Models;

namespace SirinEngineering.Controllers;

public class ProductController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult ProductList()
    {
        return View();
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
