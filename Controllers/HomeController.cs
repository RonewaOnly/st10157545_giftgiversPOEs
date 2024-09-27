using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Models;
using System.Diagnostics;

namespace st10157545_giftgiversPOEs.Controllers
{
    //[Authorize(Policy = "UserPolicy")]

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
            var relief = _context.ReliefProjects.ToList();  
            var events = _context.Events.ToList();
            var homeviewcontroller = new HomeViewController() { 
                ReliefProjects = relief,
                Events = events
            };
            return View(homeviewcontroller);
        }

        public IActionResult AboutUs()
        {
            return View();
        }
        public IActionResult Donations()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpPost]
        public IActionResult SubmitReport(string username, string disaster_type, string description,string location, string additionalDetails, string severity_level,bool is_anonymous)
        {
            var user = _context.Users.Where(user=> user.username == username);
            var report = new Report();
            int  id =0;
            if (user != null) {
                foreach (var i in user)
                {
                    id = i.User_id;
                }
            }
            if (is_anonymous)
            {
                report = new Report
                {
                    user_id = null,
                    disaster_type = disaster_type,
                    description = description,
                    location = null, // You can set this based on user input or user data
                    is_anonymous = is_anonymous,
                    report_date = DateTime.Now,
                    image_url = "https://th.bing.com/th/id/OIP.ArNBZx8vVdz6fDTOLNBanAHaEo?rs=1&pid=ImgDetMain",
                    severity_level = (int?)Convert.ToInt64(severity_level), // Default value; you can change based on user input
                };
            }
             report = new Report
            {
                user_id = id,
                disaster_type = disaster_type,
                description = description,
                location = location, // You can set this based on user input or user data
                is_anonymous = is_anonymous,
                report_date = DateTime.Now,
                image_url = "https://th.bing.com/th/id/OIP.3IsXMskZyheEWqtE3Dr7JwHaGe?rs=1&pid=ImgDetMain",
                severity_level = (int?)Convert.ToInt64(severity_level), // Default value; you can change based on user input
            };

            Console.WriteLine($"isAnonymous: {is_anonymous} value ");

        _context.Reports.Add(report);
            _context.SaveChanges();

            return RedirectToAction("Report");
        }
        public IActionResult EventDetails(int id)
        {
            // Fetch the event based on the provided ID
            var selectedEvent = _context.Events.FirstOrDefault(e => e.event_id == id);

            if (selectedEvent == null)
            {
                return NotFound(); // Return a 404 if the event is not found
            }

            // Return the event details to the view
            return View(selectedEvent);
        }

        public IActionResult Report()
        {
            var reportModel = _context.Reports.Include(r => r.User).Include(r =>r.Comments).DefaultIfEmpty().ToList();
            return View(reportModel);
        }

        [HttpPost]
        public IActionResult AddComment(int reportId, string commentContent)
        {
            if (string.IsNullOrEmpty(commentContent))
            {
                ModelState.AddModelError("", "Comment cannot be empty.");
                return RedirectToAction("Report");
            }

            var comment = new Comment
            {
                report_id = reportId,
                Content = commentContent,
                user_id = 1 // get logged-in user ID
    };

            _context.Comments.Add(comment);
            _context.SaveChanges();

            return RedirectToAction("Report");
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
