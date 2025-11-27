using Ecommerce_Webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Webapp.Controllers
{

    public class AccountController : Controller
    {
        private readonly EcommerceContext _context;
        private readonly JwtTokenService _jwt;

        public AccountController(EcommerceContext context, JwtTokenService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        // ------------------ LOGIN ------------------
        [HttpGet]
        public IActionResult Login() => View();

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

            if (user != null)
            {
                var token = _jwt.GenerateToken(user.Username);

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid login";
            return View();
        }

        // ------------------ REGISTER ------------------
        [HttpGet]
        public IActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password)
        {
            // Check if username already exists
            var exists = await _context.Users.AnyAsync(u => u.Username == username);
            if (exists)
            {
                ViewBag.Error = "Username already exists";
                return View();
            }

            var user = new User
            {
                Username = username,
                Password = password // For production, hash passwords!
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            ViewBag.Success = "Registration successful! You can now log in.";
            return View();
        }

        // ------------------ LOGOUT ------------------
        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
    }
    
}
