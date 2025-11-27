using Ecommerce_Webapp.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace Ecommerce_App.Controllers
{
    public class ProductsController : Controller
    {
        private readonly EcommerceContext _context;

        public ProductsController(EcommerceContext context)
        {
            _context = context;
        }
        [Authorize(Roles = "User")]
        public IActionResult Index()
        {
            var products = _context.Products.ToList();
            return View(products);
        }

        [Authorize(Roles = "User")]
        public async Task<IActionResult> Details(int id)
        {

            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product == null)
            {
                return NotFound();
            }
           

            return View(product);

        }
    
    }
}
