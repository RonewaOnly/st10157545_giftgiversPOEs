using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using st10157545_giftgiversPOEs.Controllers;
using st10157545_giftgiversPOEs.Models;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class AdminController : Controller
    {
        private readonly DatabaseController _context;
        public AdminController(DatabaseController context) 
        {
            _context = context;
        
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Donations()
        {
            return View();
        }
        public IActionResult Events()
        {
            var events = _context.Events.Where(e => e.event_type == "Events").ToList();
            var programs = _context.Events.Where(e => e.event_type == "Program").ToList();

            var viewModel = new EventsViewModel
            {
                CurrentEvents = events,
                CurrentPrograms = programs
            };

            return View(viewModel);
        }
        [HttpGet]
        public IActionResult CreateEvent()
        {
            var viewModel = new CreateEventViewModel();

            // Initialize AdminList when the form is loaded (GET request)
            try
            {
                var admins = _context.Admins.ToList();
                viewModel.AdminList = admins.Select(a => new SelectListItem
                {
                    Value = a.Admin_Id.ToString(),
                    Text = $"{a.Firstname} {a.Lastname}"
                }).ToList();

                if (!viewModel.AdminList.Any())
                {
                    viewModel.AdminError = "No admins available. Please create an admin first.";
                }
            }
            catch (Exception ex)
            {
                viewModel.AdminError = "An error occurred while fetching admins. Please try again later.";
                Console.WriteLine($"Error fetching admins: {ex.Message}");
            }

            return View(viewModel);
        }

        [HttpPost]
        public IActionResult CreateEvent(CreateEventViewModel viewModel)
        {
            // Custom validation for startDate and endDate
            if (viewModel.Event.startDate > viewModel.Event.endDate)
            {
                ModelState.AddModelError("Event.startDate", "Start date must be before end date.");
            }

            // Custom validation for event_date between startDate and endDate
            if (viewModel.Event.event_date < viewModel.Event.startDate || viewModel.Event.event_date > viewModel.Event.endDate)
            {
                ModelState.AddModelError("Event.event_date", "Event date must be between start and end dates.");
            }

            // Ensure admin_id is selected
            if (string.IsNullOrEmpty(viewModel.Event.admin_id))
            {
                ModelState.AddModelError("Event.admin_id", "Please select an admin.");
            }

            // If ModelState is valid, proceed to save the event
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the selected admin from the database based on admin_id
                    var selectedAdmin = _context.Admins.Find(viewModel.Event.admin_id);

                    if (selectedAdmin == null)
                    {
                        ModelState.AddModelError("Event.admin_id", "Selected admin not found.");
                    }
                    else
                    {
                        // Associate the admin with the event
                        viewModel.Event.Admin = selectedAdmin;

                        // Reset event_id to 0 to ensure a new record is created
                        viewModel.Event.event_id = 0;

                        // Add and save the new event
                        _context.Events.Add(viewModel.Event);
                        _context.SaveChanges();

                        return RedirectToAction("Events");
                    }
                }
                catch (Exception ex)
                {
                    // Log exception and display a user-friendly error
                    ModelState.AddModelError("", "An error occurred while saving the event. Please try again.");
                    Console.WriteLine($"Error while saving the event: {ex.Message}");
                }
            }

            // If ModelState is invalid, repopulate the AdminList and handle errors
            try
            {
                var admins = _context.Admins.ToList();
                viewModel.AdminList = admins.Select(a => new SelectListItem
                {
                    Value = a.Admin_Id.ToString(),
                    Text = a.Firstname + " " + a.Lastname
                }).ToList();

                // If no admins are available
                if (!viewModel.AdminList.Any())
                {
                    viewModel.AdminError = "No admins available. Please create an admin first.";
                }
            }
            catch (Exception ex)
            {
                // Log exception and set AdminError
                viewModel.AdminError = "An error occurred while fetching admins. Please try again.";
                Console.WriteLine($"Error fetching admins: {ex.Message}");
            }

            // Log the invalid ModelState errors (optional but useful for debugging)
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }

            // Return the view with the validation errors and populated AdminList
            return View(viewModel);
        }


        public IActionResult EventDetails(string id)
        {
            var eventDetails = _context.Events.Find(id);
            if (eventDetails == null)
            {
                return NotFound();
            }
            return View(eventDetails);
        }
        public IActionResult Reports()
        {
            var reports =  _context.Reports;
            return View(reports);
        }
        public IActionResult Details(int id)
        {
            var report = _context.Reports.Find(id);
            if (report == null)
            {
                return NotFound();
            }
            return View(report);
        }
        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            var report = _context.Reports.Find(id);
            if (report == null)
            {
                return NotFound();
            }
            report.status = status;
            _context.SaveChanges();
            return RedirectToAction("Reports");
        }
        [HttpPost]
        public IActionResult Delete(int id)
        {
            var report = _context.Reports.Find(id);
            if (report == null)
            {
                return NotFound();
            }
            _context.Reports.Remove(report);
            _context.SaveChanges();
            return RedirectToAction("Reports");
        }


        
        public IActionResult Profile()
        {
            return View();
        }
        public IActionResult Volunteers()
        {
            var volunteers = _context.Volunteers.ToList();
            var projects = _context.ReliefProjects.ToList();

            var viewModel = new VolunteerViewModel
            {
                Volunteers = volunteers,
                ProjectsList = new SelectList(projects, "ReliefId", "ProjectName"),
                VolunteersList = new SelectList(volunteers, "Volunteer_Id", "Firstname") // Adjust as needed
            };

            return View(viewModel);
        }

        public IActionResult Volunteer_Details(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer == null) return NotFound();
            return View(volunteer);
        }

        public IActionResult Volunteer_Edit(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer == null) return NotFound();
            return View(volunteer);
        }

        [HttpPost]
        public IActionResult Volunteer_Delete(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer == null) return NotFound();

            _context.Volunteers.Remove(volunteer);
            _context.SaveChanges();
            return RedirectToAction("Volunteers");
        }

        public IActionResult Volunteer_Assign(int volunteerId)
        {
            var events = _context.Events.ToList();
            ViewBag.Events = new SelectList(events, "event_id", "event_name");
            return View(volunteerId);
        }

        [HttpPost]
        public IActionResult AssignVolunteersToProjects(int selectedVolunteerId, int selectedProjectId)
        {
            var project = _context.ReliefProjects.Find(selectedProjectId);
            if (project != null)
            {
                project.volunteer_id = selectedVolunteerId;
                _context.SaveChanges();
            }

            return RedirectToAction("Volunteers");
        }


        public IActionResult Responses()
        {
            return View();
        }
    }
}
