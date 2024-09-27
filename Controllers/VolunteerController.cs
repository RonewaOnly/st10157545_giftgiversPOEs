using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Models;
using st10157545_giftgiversPOEs.Services;
using System.Security.Claims;
using System.Threading.Tasks;

namespace st10157545_giftgiversPOEs.Controllers
{
    //[Authorize(Policy = "VolunteerPolicy")]

    public class VolunteerController(
DatabaseController context,
GuardianNewsService newsService,
TwitterService twitterService,
InstagramService instagramService,
FacebookService facebookService,
NotificationService notificationService,
ILogger<VolunteerController> logger) : Controller
    {
        private readonly ILogger<VolunteerController> _logger = logger;
        private readonly DatabaseController _context = context;

        private readonly GuardianNewsService _newsService = newsService;
        private readonly TwitterService _twitterService = twitterService;
        private readonly InstagramService _instagramService = instagramService;
        private readonly FacebookService _facebookService = facebookService;
        private readonly NotificationService _notificationService = notificationService;


        public IActionResult Index()
        {
            var reliefProjects = _context.ReliefProjects.ToList();
            var events = _context.Events.ToList();

            var viewModel = new HomeViewModel
            {
                ReliefProjects = reliefProjects,
                Events = events
            };

            return View(viewModel);
        }

        public async Task<IActionResult> Notifications()
        {
            var userFound = _context.Volunteers
                .Include(vol => vol.ReliefProjects)
                .FirstOrDefault(vol => vol.username == User.Identity.Name);
            var userId = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;
            var notifications = await _notificationService.GetUnreadNotificationsAsync(userFound.volunteer_id.ToString());
            Console.WriteLine($"Are notifications: {notifications.Count}, userid: {userFound}");
            return View(notifications);
        }


        [HttpPost]
        public async Task<IActionResult> MarkAsRead(int id)
        {
            await _notificationService.MarkAsReadAsync(id);
            return Ok();
        }
        public IActionResult Events()
        {

            var events = _context.Events.ToList();
            return View(events);
        }

        public async Task<IActionResult> News(string? countryFilter = null, string? twitterNextToken = null, string? instagramNextPage = null, string? facebookNextPage = null)
        {
            var viewModel = new CombinedNewsViewModel();
            var news = await _newsService.GetLatestNewsAsync();

            if (news == null)
            {
                _logger.LogError("Guardian API returned null.");
            }
            else if (news.Response?.Results == null || !news.Response.Results.Any())
            {
                _logger.LogError("Guardian API returned no results.");
            }

            // Fetch filtered and paginated data
            viewModel.GuardianNews = news;
            viewModel.TwitterNews = await _twitterService.SearchDisasterNewsAsync(country: countryFilter, nextToken: twitterNextToken);
            viewModel.InstagramMedia = await _instagramService.GetDisasterMediaAsync(nextPageUrl: instagramNextPage);
            viewModel.FacebookPosts = await _facebookService.GetDisasterPostsAsync(nextPageUrl: facebookNextPage);

            // Store the next page tokens for pagination
            viewModel.TwitterNextPageToken = viewModel.TwitterNews?.Meta?.NextToken;
            viewModel.InstagramNextPageUrl = viewModel.InstagramMedia?.Paging?.Next;
            viewModel.FacebookNextPageUrl = viewModel.FacebookPosts?.Paging?.Next;

            //var model = new NewsViewModel
            //{
            //    GuardianNews = news,
            //    TwitterNews = twitterNews,
            //    InstagramMedia = instagramMedia,
            //    FacebookPosts = facebookPosts
            //};
            return View(viewModel);
        }
        public IActionResult Profile()
        {
            var userFound = _context.Volunteers
                .Include(vol => vol.ReliefProjects)
                .FirstOrDefault(vol => vol.username == User.Identity.Name);

            if (userFound == null)
            {
                // If the user is not found, redirect to an error page or login
                TempData["Error"] = "User not found. Please log in again.";
                return RedirectToAction("Login"); // Or handle this scenario as appropriate
            }

            return View(userFound);
        }
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(string username, string email, bool status, IFormFile image_url)
        {
            var currentUser = _context.Volunteers.FirstOrDefault(v => v.username == User.Identity.Name);

            if (currentUser != null)
            {
                // Update the user's profile details
                currentUser.username = username;
                currentUser.email = email;
                currentUser.Status = status; // Update the boolean status

                // Handle the image file if it was uploaded
                if (image_url != null && image_url.Length > 0)
                {
                    // Generate a unique filename and save the file
                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + image_url.FileName;
                    var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/profiles");
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    // Create the directory if it doesn't exist
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await image_url.CopyToAsync(fileStream);
                    }

                    // Store the relative path to the image in the database
                    currentUser.image_url = "/images/profiles/" + uniqueFileName;
                }

                // Save changes to the database
                _context.SaveChanges();

                // Update the user claims in the current session
                await UpdateUserClaims(currentUser);

                TempData["Success"] = "Profile updated successfully!";
            }

            return RedirectToAction("Profile");
        }



        public IActionResult Tasks()
        {
            var reliefProjects = _context.ReliefProjects.
                Include(rp=> rp.Volunteer).
                DefaultIfEmpty().
                ToList(); // Fetch all relief projects
            return View(reliefProjects);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignVolunteer(int projectId)
        {
            // Retrieve the current user's username (assuming the username is the same as the volunteer's identifier)
            var currentUserName = User.Identity.Name;

            // Find the volunteer based on the logged-in user's name
            var volunteer = await _context.Volunteers.FirstOrDefaultAsync(v => v.username == currentUserName);
            if (volunteer == null)
            {
                return NotFound("Volunteer not found.");
            }

            // Find the relief project by its ID
            var reliefProject = await _context.ReliefProjects.FindAsync(projectId);
            if (reliefProject == null)
            {
                return NotFound("Relief project not found.");
            }



            // Assign the current volunteer to the relief project
            reliefProject.volunteer_id = volunteer.volunteer_id;

            // Update the project in the database
            _context.Update(reliefProject);
            await _context.SaveChangesAsync();

            // Redirect to a suitable view (e.g., back to the list of tasks or a confirmation page)
            return RedirectToAction("Tasks");  // You can redirect to "Tasks" or any other page
        }

        private async Task UpdateUserClaims(Volunteers user)
        {
            // Remove the old identity
            await HttpContext.SignOutAsync();

            // Create a new claims identity with updated info
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.username),
        new Claim(ClaimTypes.Email, user.email),
        new Claim("UserType", user.UserType.ToString()),
        new Claim(ClaimTypes.NameIdentifier, user.volunteer_id.ToString())
    };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            // Sign the user back in with updated claims
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        }

    }
}
