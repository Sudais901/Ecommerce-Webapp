using Ecommerce_Webapp.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Ecommerce_Webapp.Controllers
{
    public class CartController : Controller
    {
        private readonly EcommerceContext _context;

        public CartController(EcommerceContext context)
        {
            _context = context;
        }

        public IActionResult AddToCart(int id)
        {
            // 1. Get Product From DB
            var product = _context.Products
                .Where(p => p.ProductId == id)
                .Select(p => new CartItem
                {
                    ProductId = p.ProductId,
                    Pname = p.Pname,
                    ImagePath = p.ImagePath,
                    Price = p.Price ?? 0,
                    Quantity = 1
                })
                .FirstOrDefault();

            if (product == null)
                return NotFound();

            // 2. Get Existing Cart from Session
            var cart = GetCart();

            // 3. Check if product already exists
            var existing = cart.FirstOrDefault(x => x.ProductId == id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                cart.Add(product);
            }

            // 4. Save Back to Session
            SaveCart(cart);
            return RedirectToAction("Index", "Cart");
        }

        public IActionResult Index()
        {
            var cart = GetCart();
            return View(cart);
        }

        // Helper Methods -----------------------------------

        private List<CartItem> GetCart()
        {
            var cartJson = HttpContext.Session.GetString("CART");

            if (string.IsNullOrEmpty(cartJson))
                return new List<CartItem>();

            return JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
        }

        private void SaveCart(List<CartItem> cart)
        {
            HttpContext.Session.SetString("CART", JsonConvert.SerializeObject(cart));
        }
        public IActionResult RemoveFromCart(int id)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
            }

            return RedirectToAction("Index");
        }
        public IActionResult IncreaseQuantity(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }

        public IActionResult DecreaseQuantity(int id)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(x => x.ProductId == id);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);

                SaveCart(cart);
            }
            return RedirectToAction("Index");
        }



    }


}
