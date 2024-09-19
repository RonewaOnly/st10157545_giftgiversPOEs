using Microsoft.AspNetCore.Mvc;
using st10157545_giftgiversPOEs.Models;
using System.Diagnostics;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DatabaseController _context;

        public HomeController(ILogger<HomeController> logger, DatabaseController context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Donations()
        {
            return View();
        }

        public IActionResult Report()
        {
            var reportModel = _context.Reports.ToList();
            return View(reportModel);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
