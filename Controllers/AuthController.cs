using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using st10157545_giftgiversPOEs.Models;
using st10157545_giftgiversPOEs.Services;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace st10157545_giftgiversPOEs.Controllers
{
    public class AuthController : Controller
    {

        private readonly DatabaseController _context;
        private readonly SessionService _sessionService;
        private readonly NotificationService _notificationService;

        public AuthController(DatabaseController context, SessionService sessionService, NotificationService notificationService)
        {
            _context = context;
            _sessionService = sessionService;
            _notificationService = notificationService;
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
                if (user != null && VerifyPassword(model.Password, user.password))
                {
                    var accessToken = await _sessionService.GenerateAccessTokenAsync(user.username);

                    var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.username),
                new Claim(ClaimTypes.NameIdentifier, user.phone),
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
                }else if (user == null)
                {
                    ModelState.AddModelError(string.Empty, "User not found.");
                    return View(model);
                }
                foreach (var claim in User.Claims)
                {
                    Console.WriteLine($"{claim.Type}: {claim.Value}");
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
                    ModelState.AddModelError("username", "username is already taken.");
                    return View(model);
                }

                if (await IsEmailTaken(model.Email))
                {
                    ModelState.AddModelError("email", "email is already in use.");
                    return View(model);
                }
                if (await IsPhoneTaken(model.Phone))
                {
                    ModelState.AddModelError("phone", "phone is already in use.");
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
                .Where(u => u.username == usernameOrEmail || u.email == usernameOrEmail)
                .Select(u => new Users
                {
                    username = u.username,
                    email = u.email,
                    phone = u.phone,
                    password = u.password
                })
                .FirstOrDefaultAsync();

            if (user != null) return user;

            var admin = await _context.Admins
                .Where(a => a.username == usernameOrEmail || a.email == usernameOrEmail)
                .Select(a => new Admins
                {
                    admin_id = a.admin_id,
                    username = a.username,
                    email = a.email,
                    phone = a.phone,
                    password = a.password,
                })
                .FirstOrDefaultAsync();

            if (admin != null) return admin;

            var volunteer = await _context.Volunteers
                .Where(v => v.username == usernameOrEmail || v.email == usernameOrEmail)
                .Select(v => new Volunteers
                {
                    volunteer_id = v.volunteer_id,
                    username = v.username,
                    email = v.email,
                    phone = v.phone,
                    password = v.password
                })
                .FirstOrDefaultAsync();

            if (volunteer != null) return volunteer;

            return null;
        }

        private async Task<bool> IsUsernameTaken(string username)
        {
            return await _context.Users.AnyAsync(u => u.username == username) ||
                   await _context.Admins.AnyAsync(a => a.username == username) ||
                   await _context.Volunteers.AnyAsync(v => v.username == username);
        }

        private async Task<bool> IsEmailTaken(string email)
        {
            return await _context.Users.AnyAsync(u => u.email == email) ||
                   await _context.Admins.AnyAsync(a => a.email == email) ||
                   await _context.Volunteers.AnyAsync(v => v.email == email);
        }

        private async Task<bool> IsPhoneTaken(string phone)
        {
            return await _context.Users.AnyAsync(u => u.phone == phone) ||
                   await _context.Admins.AnyAsync(a => a.phone == phone) ||
                   await _context.Volunteers.AnyAsync(v => v.phone == phone);
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
                        username = model.Username,
                        email = model.Email,
                        phone = model.Phone,
                        Gender = model.Gender,
                        password = HashPassword(model.password)
                    };
                case UserType.Admin:
                    return new Admins
                    {
                        admin_id = Guid.NewGuid().ToString(),
                        firstname = model.FirstName,
                        lastname = model.LastName,
                        username = model.Username,
                        email = model.Email,
                        phone = model.Phone,
                        gender = model.Gender,
                        password = HashPassword(model.password),
                        qualifications = model.Qualifications,
                        skills = model.Skills,
                        speciality = model.Speciality
                    };
                case UserType.Volunteer:
                    return new Volunteers
                    {
                        firstname = model.FirstName,
                        lastname = model.LastName,
                        username = model.Username,
                        email = model.Email,
                        phone = model.Phone,
                        gender = model.Gender,
                        password = HashPassword(model.password),
                        skills = model.Skills,
                        age = model.Age.Value,
                        student = model.IsStudent ? 1 : 0,
                        area = model.Area

                    };
                default:
                    throw new ArgumentException("Invalid user type");
            }
        }


        private async Task LoginUser(IUser user)
        {
            var accessToken = await _sessionService.GenerateAccessTokenAsync(user.username);

            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.Name, user.username),
        new Claim(ClaimTypes.NameIdentifier, user.username),
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

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
