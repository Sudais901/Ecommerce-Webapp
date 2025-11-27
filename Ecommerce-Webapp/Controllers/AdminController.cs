using Ecommerce_Webapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce_Webapp.Controllers
{
    public class AdminController : Controller
    {
        private readonly EcommerceContext _context;
        private readonly JwtTokenService _jwt;

        public AdminController(EcommerceContext context, JwtTokenService jwt)
        {
            _context = context;
            _jwt = jwt;
        }

        [HttpGet]
        public IActionResult Login() => View(); // Admin login view

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password)
        {
            var admin = await _context.Admins
                .FirstOrDefaultAsync(a => a.Username == username && a.Password == password);

            if (admin != null)
            {
                var token = _jwt.GenerateToken(admin.Username, "Admin"); // <-- add role

                Response.Cookies.Append("jwt", token, new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddHours(2)
                });

                return RedirectToAction("Dashboard");
            }

            ViewBag.Error = "Invalid login";
            return View();
        }
        [Authorize(Roles = "Admin")]
        public IActionResult Dashboard()
        {
            // Here you can decode JWT to ensure role is Admin
            return View();
        }
        // ------------------ ADD PRODUCT ------------------
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddProduct()
        {
            if (!IsAdmin()) return RedirectToAction("Login");
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> AddProduct(AddProductViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Create product
            var product = new Product
            {
                Pname = model.Pname,
                Description = model.Description,
                Price = model.Price
            };

            _context.Products.Add(product);
            await _context.SaveChangesAsync();  // Get ProductId

            // Folder path
            string folder = Path.Combine("wwwroot", "Images");
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            foreach (var img in model.Images)
            {
                if (img != null)
                {
                    string fileName = Guid.NewGuid() + Path.GetExtension(img.FileName);
                    string fullPath = Path.Combine(folder, fileName);

                    using (var stream = new FileStream(fullPath, FileMode.Create))
                    {
                        await img.CopyToAsync(stream);
                    }

                    string dbPath = "/Images/" + fileName;

                    // 1️⃣ Save ALL images in ProductImages table
                    var pimg = new ProductImage
                    {
                        ProductId = product.ProductId,
                        ImagePath = dbPath
                    };

                    _context.ProductImages.Add(pimg);

                    //  Save FIRST image as main product image
                    if (string.IsNullOrEmpty(product.ImagePath))
                    {
                        product.ImagePath = dbPath;
                    }
                }
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Dashboard");
        }


        public IActionResult Logout()
        {
            Response.Cookies.Delete("jwt");
            return RedirectToAction("Login");
        }
        private bool IsAdmin()
        {
            var jwt = Request.Cookies["jwt"];
            if (string.IsNullOrEmpty(jwt)) return false;

            var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var token = handler.ReadJwtToken(jwt);

            var role = token.Claims.FirstOrDefault(c => c.Type == System.Security.Claims.ClaimTypes.Role)?.Value;
            return role == "Admin";
        }
    }
}


