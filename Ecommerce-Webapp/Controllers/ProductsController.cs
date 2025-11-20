using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Ecommerce_App.Controllers
{
    public class ProductsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }


        public IActionResult Details(int id)
        {
            var products = new List<dynamic>
    {
        new { Id = 1, Name = "Product 1", Img = "product1.jpg", Desc = " This is Product 1 description. It’s high quality and best in class.",price=499.00 },
        new { Id = 2, Name = "Product 2", Img = "product2.jpg", Desc = " This is Product 2 description. It’s stylish, powerful, and affordable.,",price=499.00  },
        new { Id = 3, Name = "Product 3", Img = "product3.jpg", Desc = " This is Product 3 description. It’s durable, modern, and top-rated.",price=899.00 }
    };


            // Sample products list in ViewBag
            var product = products.FirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return NotFound(); // Or redirect to a page saying "Product not found"
            }

            // Pass the selected product to the view
            return View(product);
        }
    }
}
