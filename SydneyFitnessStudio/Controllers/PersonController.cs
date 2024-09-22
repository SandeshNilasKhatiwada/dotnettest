using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SydneyFitnessStudio.Models;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SydneyFitnessStudio.Controllers
{
    public class PersonController : Controller
    {
        private readonly FitnessStudioContext _context;

        public PersonController(FitnessStudioContext context)
        {
            _context = context;
        }

        // Register a new person (Client or Staff)
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(Person person)
        {
            if (ModelState.IsValid)
            {
                // Encrypt the password for security
                person.Password = BCrypt.Net.BCrypt.HashPassword(person.Password);
                _context.Add(person);
                await _context.SaveChangesAsync();
                return RedirectToAction("Login");
            }
            return View(person);
        }

        // Login logic
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string email, string password)
        {
            var person = await _context.People.FirstOrDefaultAsync(p => p.Email == email);
            if (person != null && BCrypt.Net.BCrypt.Verify(password, person.Password))
            {
                // Create the user claims for cookie authentication
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, person.Email),
                    new Claim(ClaimTypes.Role, person.Role) // Add role claim (Client or Staff)
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                // Create and sign in user with cookies
                var authProperties = new AuthenticationProperties
                {
                    IsPersistent = true // Remember the user across sessions
                };

                await HttpContext.SignInAsync(
                    CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity),
                    authProperties);

                return RedirectToAction("Dashboard");
            }

            ModelState.AddModelError("", "Invalid login attempt.");
            return View();
        }

        // Logout logic
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }

        // Dashboard for both Clients and Staff (Protected)
        [HttpGet]
        [Authorize] // Ensure only authenticated users can access this
        public IActionResult Dashboard()
        {
            // Logic to display different data based on user role
            if (User.IsInRole("Staff"))
            {
                // Staff-specific logic here
            }
            else if (User.IsInRole("Client"))
            {
                // Client-specific logic here
            }

            return View();
        }
    }
}
