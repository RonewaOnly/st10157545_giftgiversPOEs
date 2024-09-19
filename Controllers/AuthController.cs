using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using st10157545_giftgiversPOEs.Models;
using System.Security.Claims;
using System.Text;
using System;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class AuthController : Controller
    {

        private readonly DatabaseController  _context;
        private readonly SessionService _sessionService;

        public AuthController(DatabaseController context, SessionService sessionService)
        {
            _context = context;
            _sessionService = sessionService;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await FindUserByUsernameOrEmail(model.UsernameOrEmail);
                if (user != null && VerifyPassword(model.Password, user.Password))
                {
                    var accessToken = await _sessionService.GenerateAccessTokenAsync(user.Username);

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Phone),
                new Claim("AccessToken", accessToken),
                new Claim("UserType", user.UserType.ToString())
            };

                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {
                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                // Log validation errors for debugging (optional)
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                Console.WriteLine(string.Join(", ", errors));

                // Show validation errors and prevent data loss
                return View(model);
            }


            if (ModelState.IsValid)
            {
                if (await IsUsernameTaken(model.Username))
                {
                    ModelState.AddModelError("Username", "Username is already taken.");
                    return View(model);
                }

                if (await IsEmailTaken(model.Email))
                {
                    ModelState.AddModelError("Email", "Email is already in use.");
                    return View(model);
                }
                if (await IsPhoneTaken(model.Phone))
                {
                    ModelState.AddModelError("Phone", "Phone is already in use.");
                    return View(model);
                }

                var user = CreateUserFromModel(model);

                await _context.AddAsync(user);
                await _context.SaveChangesAsync();

                // Automatically log in the user after registration
                await LoginUser(user);

                return RedirectToAction("Index", "Home");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            var accessToken = User.Claims.FirstOrDefault(c => c.Type == "AccessToken")?.Value;

            if (!string.IsNullOrEmpty(accessToken))
            {
                await _sessionService.InvalidateTokenAsync(accessToken);
            }
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Index", "Home");
        }

        private async Task<IUser> FindUserByUsernameOrEmail(string usernameOrEmail)
        {
            var user = await _context.Users
                .Where(u => u.Username == usernameOrEmail || u.Email == usernameOrEmail)
                .Select(u => new Users
                {
                    Username = u.Username,
                    Email = u.Email,
                    Phone = u.Phone,
                    Password = u.Password
                })
                .FirstOrDefaultAsync();

            if (user != null) return user;

            var admin = await _context.Admins
                .Where(a => a.Username == usernameOrEmail || a.Email == usernameOrEmail)
                .Select(a => new Admins
                {
                    Admin_Id = a.Admin_Id,
                    Username = a.Username,
                    Email = a.Email,
                    Phone = a.Phone,
                    Password = a.Password,
                })
                .FirstOrDefaultAsync();

            if (admin != null) return admin;

            var volunteer = await _context.Volunteers
                .Where(v => v.Username == usernameOrEmail || v.Email == usernameOrEmail)
                .Select(v => new Volunteers
                {
                    Volunteer_Id = v.Volunteer_Id,
                    Username = v.Username,
                    Email = v.Email,
                    Phone = v.Phone,
                    Password = v.Password
                })
                .FirstOrDefaultAsync();

            if (volunteer != null) return volunteer;

            return null;
        }

        private async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username == username) ||
                   await _context.Admins.AnyAsync(a => a.Username == username) ||
                   await _context.Volunteers.AnyAsync(v => v.Username == username);
        }

        private async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email == email) ||
                   await _context.Admins.AnyAsync(a => a.Email == email) ||
                   await _context.Volunteers.AnyAsync(v => v.Email == email);
        }

        private async Task<bool> IsPhoneTaken(string phone)
        {
            return await _context.Users.AnyAsync(u => u.Phone == phone) ||
                   await _context.Admins.AnyAsync(a => a.Phone == phone) ||
                   await _context.Volunteers.AnyAsync(v => v.Phone == phone);
        }

        private IUser CreateUserFromModel(RegisterViewModel model)
        {
            switch (model.UserType)
            {
                case UserType.User:
                    return new Users
                    {
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Username = model.Username,
                        Email = model.Email,
                        Phone = model.Phone,
                        Gender = model.Gender,
                        Password = HashPassword(model.Password)
                    };
                case UserType.Admin:
                    return new Admins
                    {
                        Admin_Id = Guid.NewGuid().ToString(),
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Username = model.Username,
                        Email = model.Email,
                        Phone = model.Phone,
                        Gender = model.Gender,
                        Password = HashPassword(model.Password),
                        Qualifications = model.Qualifications,
                        Skills = model.Skills,
                        Speciality = model.Speciality
                    };
                case UserType.Volunteer:
                    return new Volunteers
                    {
                        Firstname = model.FirstName,
                        Lastname = model.LastName,
                        Username = model.Username,
                        Email = model.Email,
                        Phone = model.Phone,
                        Gender = model.Gender,
                        Password = HashPassword(model.Password),
                        Skills = model.Skills,
                        Age = model.Age.Value,
                        Student = model.IsStudent ? 1 : 0,
                        Area = model.Area

                    };
                default:
                    throw new ArgumentException("Invalid user type");
            }
        }


        private async Task LoginUser(IUser user)
        {
            var accessToken = await _sessionService.GenerateAccessTokenAsync(user.Username);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.Username),
        new Claim(ClaimTypes.NameIdentifier, user.Username),
        new Claim("UserType", user.UserType.ToString()),
         new Claim("AccessToken", accessToken),

    };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties
            {
                IsPersistent = true
            };

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity),
                authProperties);
        }

        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
            }
        }

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var hashOfEnteredPassword = HashPassword(enteredPassword);
            return storedHash.Equals(hashOfEnteredPassword, StringComparison.OrdinalIgnoreCase);
        }

    }
}
