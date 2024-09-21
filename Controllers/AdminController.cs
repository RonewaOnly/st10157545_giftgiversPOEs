using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
            var donations = _context.Donations
       .Include(d => d.Event)
       .AsNoTracking()
       .ToList();
            return View(donations);
        }
        public IActionResult Donation_Add()
        {
            // Load admins for the dropdown
            var admins = _context.Admins.Select(a => new SelectListItem
            {
                Value = a.admin_id, // Use the admin_id as the value
                Text = a.firstname + " " + a.lastname // Display full name in the dropdown
            }).ToList();

            // Create the view model and populate the AdminList
            var viewModel = new DonationViewModel
            {
                AdminList = admins // Pass the list of admins to the view model
            };

            return View(viewModel);
        }

        // POST: Donation_Add
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Donation_Add(DonationViewModel model)
        {
            // Reload the AdminList in case of form validation errors
            model.AdminList = _context.Admins.Select(a => new SelectListItem
            {
                Value = a.admin_id,
                Text = a.firstname + " " + a.lastname
            }).ToList();
            ModelState.Remove("AdminList");

            if (ModelState.IsValid)
            {
                // Create the Donation entity from the view model data
                var donation = new Donation
                {
                    donation_id = model.DonationId,        // Already generated in ViewModel constructor
                    item_name = model.ItemName,
                    item_category = model.ItemCategory,
                    item_quantity = model.ItemQuantity,
                    donation_date = model.DonationDate,
                    cash_amount = model.CashAmount,
                    fullnameDonator = model.FullNameDonator,
                    admin_id = model.AdminId, // Admin is selected from the dropdown
                                              // Set other fields as necessary
                };

                _context.Donations.Add(donation);
                _context.SaveChanges();

                return RedirectToAction("Donations");
            }
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }

            // If model state is invalid, return the view with validation errors
            return View(model);
        }

        public IActionResult Donation_Edit(string id)
        {
            var donation = _context.Donations.Find(id);
            if (donation == null) return NotFound();

            // Load admins for the dropdown
            var admins = _context.Admins.Select(a => new SelectListItem
            {
                Value = a.admin_id,
                Text = a.firstname + " " + a.lastname
            }).ToList();

            // Create the view model and populate the AdminList
            var model = new DonationViewModel
            {
                DonationId = donation.donation_id,
                ItemName = donation.item_name,
                ItemCategory = donation.item_category,
                ItemQuantity = donation.item_quantity,
                DonationDate = donation.donation_date,
                CashAmount = donation.cash_amount,
                FullNameDonator = donation.fullnameDonator,
                AdminId = donation.admin_id,
                VolunteerId = donation.volunteer_id,
                AdminList = admins // Pass the list of admins to the view model
            };
            return View(model);
        }

        // POST: Donation_Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Donation_Edit(DonationViewModel model)
        {
            // Reload the AdminList in case of form validation errors
            model.AdminList = _context.Admins.Select(a => new SelectListItem
            {
                Value = a.admin_id,
                Text = a.firstname + " " + a.lastname
            }).ToList();
            ModelState.Remove("AdminList");

            if (ModelState.IsValid)
            {
                var donation = _context.Donations.Find(model.DonationId);
                if (donation != null)
                {
                    donation.item_name = model.ItemName;
                    donation.item_category = model.ItemCategory;
                    donation.item_quantity = model.ItemQuantity;
                    donation.donation_date = model.DonationDate;
                    donation.cash_amount = model.CashAmount;
                    donation.fullnameDonator = model.FullNameDonator;
                    donation.admin_id = model.AdminId; // Update the selected admin

                    // Update other properties as needed

                    _context.SaveChanges();
                    return RedirectToAction("Donations");
                }
            }

            // If model state is invalid, return the view with validation errors
            return View(model);
        }

        [HttpPost]
        public IActionResult Donation_Delete(string id)
        {
            var donation = _context.Donations.Find(id);
            if (donation == null) return NotFound();

            _context.Donations.Remove(donation);
            _context.SaveChanges();
            return RedirectToAction("Donations");
        }

        public IActionResult Donation_Assign(string id)
        {
            var donation = _context.Donations.Find(id);
            if (donation == null) return NotFound();

            // Load events and volunteers for assignment
            ViewBag.Events = new SelectList(_context.Events, "event_id", "event_name");
            ViewBag.Volunteers = new SelectList(_context.Volunteers, "volunteer_id", "firstname, lastname"); // Use correct property name

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken] // Ensure CSRF protection
        public IActionResult Donation_Assign(string id, int? eventId, int? volunteerId)
        {
            var donation = _context.Donations.Find(id);
            if (donation != null)
            {
                donation.event_id = eventId;
                donation.volunteer_id = volunteerId;
                _context.SaveChanges();
                return RedirectToAction("Donations");
            }

            // Optionally handle the case where the donation is not found
            ModelState.AddModelError("", "Donation not found.");
            return View(donation); // Return the same view with an error message
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
                    Value = a.admin_id.ToString(),
                    Text = $"{a.firstname} {a.lastname}"
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

            // Remove validation for navigation properties Admin and Donations
            ModelState.Remove("Event.Admin");
            ModelState.Remove("Event.Donations");

            // Check if the ModelState is valid before proceeding
            if (ModelState.IsValid)
            {
                try
                {
                    // Fetch the selected admin based on admin_id
                    var selectedAdmin = _context.Admins.Find(viewModel.Event.admin_id);

                    if (selectedAdmin == null)
                    {
                        ModelState.AddModelError("Event.admin_id", "Selected admin not found.");
                    }
                    else
                    {
                        // Associate the admin with the event
                        viewModel.Event.Admin = selectedAdmin;

                        // Fetch and associate donations (no validation for donations)
                        viewModel.Event.Donations = _context.Donations.ToList();

                        // Ensure event_id is set to 0 for a new record
                        viewModel.Event.event_id = 0;

                        // Add and save the new event to the database
                        _context.Events.Add(viewModel.Event);
                        _context.SaveChanges();

                        return RedirectToAction("Events");
                    }
                }
                catch (Exception ex)
                {
                    // Log exception and display user-friendly error message
                    ModelState.AddModelError("", "An error occurred while saving the event. Please try again.");
                    Console.WriteLine($"Error while saving the event: {ex.Message}");
                }
            }

            // Repopulate AdminList in case of validation failure
            try
            {
                var admins = _context.Admins.ToList();
                viewModel.AdminList = admins.Select(a => new SelectListItem
                {
                    Value = a.admin_id.ToString(),
                    Text = a.firstname + " " + a.lastname
                }).ToList();

                if (!viewModel.AdminList.Any())
                {
                    viewModel.AdminError = "No admins available. Please create an admin first.";
                }
            }
            catch (Exception ex)
            {
                viewModel.AdminError = "An error occurred while fetching admins. Please try again.";
                Console.WriteLine($"Error fetching admins: {ex.Message}");
            }

            // Log ModelState errors (for debugging)
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}, Array Admin: {viewModel.AdminList.Any()}");
                }
            }

            // Return the view with the validation errors and AdminList
            return View(viewModel);
        }



        public IActionResult EventDetails(int id)
        {
            var eventDetails = _context.Events
        .Include(e => e.Admin)        // Eager load the Admin
        .Include(e => e.Donations)    // Eager load the Donations
        .FirstOrDefault(e => e.event_id == id);
            if (eventDetails == null)
            {
                return NotFound();
            }
            Console.WriteLine($"Event Table: {_context.Events.Find(id)}");
            return View(eventDetails);
        }
        [HttpGet]
        public IActionResult EditEvent(int id)
        {
            // Load the event with its Admin and Donations
            var eventDetails = _context.Events
                .Include(e => e.Admin)        // Load Admin
                .Include(e => e.Donations)    // Load Donations
                .FirstOrDefault(e => e.event_id == id);

            if (eventDetails == null)
            {
                return NotFound();
            }

            // Load all admins for the dropdown selection
            var admins = _context.Admins.ToList();
            var adminList = admins.Select(a => new SelectListItem
            {
                Value = a.admin_id.ToString(),
                Text = a.firstname + " " + a.lastname
            }).ToList();

            // Prepare ViewModel for the view
            var viewModel = new CreateEventViewModel
            {
                Event = eventDetails,
                AdminList = adminList
            };

            return View(viewModel);
        }

        // POST: EditEvent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditEvent(CreateEventViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                // If the model state is invalid, reload the AdminList for the view
                viewModel.AdminList = _context.Admins.Select(a => new SelectListItem
                {
                    Value = a.admin_id.ToString(),
                    Text = a.firstname + " " + a.lastname
                }).ToList();

                return View(viewModel);
            }

            var eventToUpdate = _context.Events
                .Include(e => e.Admin)        // Include related entities
                .FirstOrDefault(e => e.event_id == viewModel.Event.event_id);

            if (eventToUpdate == null)
            {
                return NotFound();
            }

            // Update event properties with form data
            eventToUpdate.event_name = viewModel.Event.event_name;
            eventToUpdate.description = viewModel.Event.description;
            eventToUpdate.location = viewModel.Event.location;
            eventToUpdate.image_url = viewModel.Event.image_url;
            eventToUpdate.event_type = viewModel.Event.event_type;
            eventToUpdate.startDate = viewModel.Event.startDate;
            eventToUpdate.endDate = viewModel.Event.endDate;
            eventToUpdate.event_date = viewModel.Event.event_date;
            eventToUpdate.admin_id = viewModel.Event.admin_id;

            // Save the updated event
            _context.SaveChanges();

            return RedirectToAction("EventDetails", new { id = eventToUpdate.event_id });
        }

        public IActionResult Reports()
        {
            var reports = _context.Reports.ToList();
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
                ProjectsList = new SelectList(projects, "relief_id", "projectName"),
                VolunteersList = new SelectList(volunteers, "volunteer_id", "firstname") // Adjust as needed
            };

            return View(viewModel);
        }

        public IActionResult Volunteer_Details(int id)
        {
            var volunteer = _context.Volunteers.Find(id);
            if (volunteer == null) return NotFound();
            Console.WriteLine($"Volunteer table: {volunteer}");
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


        [HttpGet]
        public async Task<IActionResult> Responses()
        {
            var reliefProjects = await _context.ReliefProjects
                .Include(rp => rp.Admin)
                .Include(rp => rp.Volunteer)
                .Include(rp => rp.Resource)
        .ToListAsync();

            return View(reliefProjects);
        }

        [HttpGet]
        public IActionResult CreateReliefProject()
        {
            ViewBag.Admins = _context.Admins.ToList();
            ViewBag.Volunteers = _context.Volunteers.ToList();
            ViewBag.Resources = _context.Resources.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateReliefProject(ReliefProject reliefProject)
        {
            ModelState.Remove("Admin");
            ModelState.Remove("Resource");
            ModelState.Remove("Volunteer");
            ModelState.Remove("resourcesUsed");
            ModelState.Remove("adminAssignedBy");
            if (ModelState.IsValid)
            {
                _context.Add(reliefProject);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Responses));
            }
            ViewBag.Admins = _context.Admins.ToList();
            ViewBag.Volunteers = _context.Volunteers.ToList();
            ViewBag.Resources = _context.Resources.ToList();

            // Log ModelState errors (for debugging)
            foreach (var key in ModelState.Keys)
            {
                var errors = ModelState[key].Errors;
                foreach (var error in errors)
                {
                    Console.WriteLine($"Field: {key}, Error: {error.ErrorMessage}");
                }
            }
            return View(reliefProject);
        }

        [HttpGet]
        public async Task<IActionResult> EditReliefProject(int id)
        {
            var reliefProject = await _context.ReliefProjects.FindAsync(id);
            if (reliefProject == null)
            {
                return NotFound();
            }
            ViewBag.Admins = _context.Admins.ToList();
            ViewBag.Volunteers = _context.Volunteers.ToList();
            ViewBag.Resources = _context.Resources.ToList();
            return View(reliefProject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReliefProject(int id, ReliefProject reliefProject)
        {
            if (id != reliefProject.relief_id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reliefProject);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReliefProjectExists(reliefProject.relief_id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Responses));
            }
            ViewBag.Admins = _context.Admins.ToList();
            ViewBag.Volunteers = _context.Volunteers.ToList();
            ViewBag.Resources = _context.Resources.ToList();
            return View(reliefProject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteReliefProject(int id)
        {
            var reliefProject = await _context.ReliefProjects.FindAsync(id);
            if (reliefProject == null)
            {
                return NotFound();
            }

            _context.ReliefProjects.Remove(reliefProject);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Responses));
        }

        private bool ReliefProjectExists(int id)
        {
            return _context.ReliefProjects.Any(e => e.relief_id == id);
        }

        [HttpGet]
        public async Task<IActionResult> AssignTeam(int id)
        {
            var reliefProject = await _context.ReliefProjects.FindAsync(id);
            if (reliefProject == null)
            {
                return NotFound();
            }
            ViewBag.Volunteers = _context.Volunteers.ToList();
            return View(reliefProject);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeam(int id, string teamLeader, List<int> teamMembers)
        {
            var reliefProject = await _context.ReliefProjects.FindAsync(id);
            if (reliefProject == null)
            {
                return NotFound();
            }

            reliefProject.teamLeader = teamLeader;
            reliefProject.teamAssigned = string.Join(",", teamMembers);

            _context.Update(reliefProject);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Responses));
        }

    }
}
