using Microsoft.AspNetCore.Mvc;

namespace SirinEngineering.Controllers
{
    public class SalesController : Controller
    {
        public IActionResult Shop()
        {
            return View();
        }
        public IActionResult StockCheck()
        {
            return View();
        }
    }
}