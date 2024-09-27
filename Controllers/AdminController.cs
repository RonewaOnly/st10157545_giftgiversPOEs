using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Models;
using st10157545_giftgiversPOEs.Services;

namespace st10157545_giftgiversPOEs.Controllers
{
    //[Authorize(Policy = "AdminPolicy")]
    public class AdminController : Controller
    {
        private readonly DatabaseController _context;

        private readonly EmailService _emailService;
        private readonly NotificationService _notificationService;

        public AdminController(DatabaseController context, EmailService emailService, NotificationService notificationService)
        {
            _context = context;
            _emailService = emailService;
            _notificationService = notificationService;
        }


        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Donations()
        {
#pragma warning disable CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
            var donations = _context.Donations
                .Include(d => d.Event)
                .DefaultIfEmpty()  
                .AsNoTracking()
                .ToList();
#pragma warning restore CS8634 // The type cannot be used as type parameter in the generic type or method. Nullability of type argument doesn't match 'class' constraint.
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
            if (donation == null)
            {
                return NotFound();
            }

            var model = new DonationViewModel
            {
                DonationId = donation.donation_id, // Make sure this is set
                ItemName = donation.item_name,
                ItemCategory = donation.item_category,
                ItemQuantity = donation.item_quantity,
                DonationDate = donation.donation_date,
                CashAmount = donation.cash_amount,
                FullNameDonator = donation.fullnameDonator,
                AdminId = donation.admin_id,
                AdminList = _context.Admins.Select(a => new SelectListItem
                {
                    Value = a.admin_id,
                    Text = a.firstname + " " + a.lastname
                }).ToList()
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
                    donation.donation_id = model.DonationId;
                    donation.item_name = model.ItemName;
                    donation.item_category = model.ItemCategory;
                    donation.item_quantity = model.ItemQuantity;
                    donation.donation_date = model.DonationDate;
                    donation.cash_amount = model.CashAmount;
                    donation.fullnameDonator = model.FullNameDonator;
                    donation.admin_id = model.AdminId; // Update the selected admin

                    try
                    {
                        _context.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error saving changes: {ex.Message}");
                    }

                    return RedirectToAction("Donations");
                }
                else
                {
                    Console.WriteLine("Donation not found.");
                    // Optionally return an error message to the view
                    return View(model);
                }
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
            ViewBag.Volunteers = new SelectList(_context.Volunteers, "volunteer_id", "firstname"); // Use correct property name

            return View(donation);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Donation_Assign(string id, int? eventId, int? volunteerId)
        {
            var donation = _context.Donations.Find(id);
            if (donation != null)
            {
                donation.event_id = eventId;
                donation.volunteer_id = volunteerId;
                await _context.SaveChangesAsync();

                var volunteer = await _context.Volunteers.FindAsync(volunteerId);
                if (volunteer != null)
                {
                    string subject = "Task Assignment Notification";
                    string body = $"Hello {volunteer.firstname},<br/>" +
                                  $"You have been assigned to a donation event.<br/>" +
                                  $"Event ID: {eventId}.<br/>" +
                                  "Please check your dashboard for more details.";

                    await _emailService.SendEmailAsync(volunteer.email, subject, body);
                }

                return RedirectToAction("Donations");
            }

            ModelState.AddModelError("", "Donation not found.");
            return View(donation);
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
            try
            {
                var eventDetails = _context.Events
                    .Include(e => e.Admin)        // Eager load the Admin
                    .Include(e => e.Donations)    // Eager load the Donations
                    .FirstOrDefault(e => e.event_id == id);

                if (eventDetails == null)
                {
                    return NotFound();
                }

                return View(eventDetails);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in EventDetails: {ex.Message}");
                return View("Error", new ErrorViewModel { RequestId = HttpContext.TraceIdentifier, Message = "An error occurred while fetching event details." });
            }
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
            try
            {
                var reports = _context.Reports.ToList();
                return View(reports);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while fetching reports. Please try again later.");
                Console.WriteLine($"Error in Reports: {ex.Message}");
                return View(); // Return an empty view with an error.
            }
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
        public async Task<IActionResult> AssignVolunteersToProjects(int selectedVolunteerId, int selectedProjectId)
        {
            var project = await _context.ReliefProjects.FindAsync(selectedProjectId);
            if (project != null)
            {
                project.volunteer_id = selectedVolunteerId;
                await _context.SaveChangesAsync();

                var admins = await _context.Admins.Select(a => a.admin_id).ToListAsync();
                var volunteers = await _context.Volunteers.Select(v => v.volunteer_id).ToListAsync();

                // Notify all admins
                foreach (var admin in admins)
                {
                    await _notificationService.CreateNotificationAsync("A volunteer has been assigned a task ", admin);
                }

                // Notify all volunteers
                foreach (var volunteer in volunteers)
                {
                    await _notificationService.CreateNotificationAsync("Volunteer an admin as assigned you in a relief project check it on task page", volunteer.ToString());
                }
            }

            return RedirectToAction("Responses");
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
            // Remove unnecessary properties from model state
            ModelState.Remove("Admin");
            ModelState.Remove("Resource");
            ModelState.Remove("Volunteer");
            ModelState.Remove("resourcesUsed");
            ModelState.Remove("adminAssignedBy");

            if (reliefProject.startDate == DateOnly.MinValue || reliefProject.endDate == DateOnly.MinValue)
            {
                ModelState.AddModelError(nameof(reliefProject.startDate), "Start date cannot be empty or invalid.");
                // You can also log the values here for further investigation.
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Add the relief project to the context
                    _context.Add(reliefProject);
                    await _context.SaveChangesAsync();

                    // Fetch admin and volunteer IDs safely
                    var admins = await _context.Admins.Select(a => a.admin_id).ToListAsync();
                    var volunteers = await _context.Volunteers.Select(v => v.volunteer_id).ToListAsync();

                    // Notify all admins
                    foreach (var admin in admins)
                    {
                        await _notificationService.CreateNotificationAsync("A new relief project has been created.", admin);
                    }

                    // Notify all volunteers
                    foreach (var volunteer in volunteers)
                    {
                        await _notificationService.CreateNotificationAsync("A new relief project is available for volunteers.", volunteer.ToString());
                    }

                    return RedirectToAction(nameof(Responses));
                }
                catch (DbUpdateException dbEx)
                {
                    // Log the error and add a model state error for the user
                    ModelState.AddModelError(string.Empty, "An error occurred while saving the relief project. Please try again.");
                    Console.WriteLine($"Database update error: {dbEx.Message}"); // Log to console or a logger
                    return RedirectToAction(nameof(Responses));

                }
                catch (Exception ex)
                {
                    // General error handling
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
                    Console.WriteLine($"Unexpected error: {ex.Message}"); // Log to console or a logger
                }
            }

            // If we get to this point, something failed; reload necessary data for the view
            ViewBag.Admins = await _context.Admins.ToListAsync();
            ViewBag.Volunteers = await _context.Volunteers.ToListAsync();
            ViewBag.Resources = await _context.Resources.ToListAsync();

            // Log model state errors for debugging purposes
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
        public IActionResult EditReliefProject(int id)
        {
            var project = _context.ReliefProjects.Find(id);
            
            if (project == null)
            {
                return NotFound();
            }

            ViewBag.Admins = _context.Admins.ToList();
            ViewBag.Volunteers = _context.Volunteers.ToList();
            ViewBag.Resources = _context.Resources.ToList();

            return View(project);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditReliefProject(int relief_id, ReliefProject reliefProject)
        {
            ModelState.Remove("Admin");
            ModelState.Remove("Resource");
            ModelState.Remove("Volunteer");

            //if (reliefProject.teamAssigned != null)
            //{
            //    if (!int.TryParse(reliefProject.teamAssigned, out int teamId))
            //    {
            //        return BadRequest("Invalid team assigned.");
            //    }
            //    // Now you can use teamId safely
            //}

            if (relief_id != reliefProject.relief_id)
            {
                return NotFound();
            }

            Console.WriteLine($"Data type of teamAssigned:{reliefProject.teamAssigned.GetType()} ");

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(reliefProject);
                    await _context.SaveChangesAsync();

                    //var volunteerIds = reliefProject.teamAssigned?.Split(',').Select(int.Parse);
                    //if (volunteerIds != null)
                    //{
                    //    foreach (var volunteerId in volunteerIds)
                    //    {
                    //        var volunteer = await _context.Volunteers.FindAsync(volunteerId);
                    //        if (volunteer != null)
                    //        {
                    //            string subject = "Relief Project Updated";
                    //            string body = $"Hello {volunteer.firstname},<br/>" +
                    //                          $"The relief project you are assigned to has been updated.<br/>" +
                    //                          $"Project ID: {reliefProject.relief_id}.<br/>" +
                    //                          "Please check your dashboard for more details.";

                    //            await _emailService.SendEmailAsync(volunteer.email, subject, body);
                    //        }
                    //    }
                    //}
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
        public IActionResult AssignTeam(int relief_id)
        {
            var reliefProject =  _context.ReliefProjects.Find(relief_id);
            if (reliefProject == null)
            {
                return NotFound();
            }

            // Ensure Volunteers data is fetched correctly
            ViewBag.Volunteers =  _context.Volunteers.ToList();
            Console.WriteLine($"Volunteer: {ViewBag.Volunteers}");

            return View(reliefProject);  // Ensure there is a view in Views/YourController/AssignTeam.cshtml
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignTeam(int relief_id, string teamLeader, List<int> teamMembers)
        {
            var reliefProject = await _context.ReliefProjects.FindAsync(relief_id);
            if (reliefProject == null)
            {
                return NotFound();
            }

            reliefProject.teamLeader = teamLeader;
            reliefProject.teamAssigned = string.Join(",", teamMembers);

            _context.Update(reliefProject);
            await _context.SaveChangesAsync();
            var admins = await _context.Admins.Select(a => a.admin_id).ToListAsync();
            var volunteers = await _context.Volunteers.Select(v => v.volunteer_id).ToListAsync();

            // Notify all admins
            foreach (var admin in admins)
            {
                await _notificationService.CreateNotificationAsync("A volunteer has been assigned a task ", admin);
            }

            // Notify all volunteers
            foreach (var volunteer in volunteers)
            {
                await _notificationService.CreateNotificationAsync("Volunteer an admin as assigned you in a relief project check it on task page", volunteer.ToString());
            }

            return RedirectToAction(nameof(Responses));
        }

    }
}
