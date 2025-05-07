using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;

namespace SportsStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IStoreRepository repository;
        private readonly Cart cart;

        public CartController(IStoreRepository repo, Cart cartService)
        {
            repository = repo;
            cart = cartService;
        }
        public ViewResult Cart(string returnUrl = "/")
        {
            return View(new CartIndexViewModel { Cart = cart, ReturnUrl = returnUrl });
        }


        [HttpPost]
        public IActionResult AddToCart(int productId, string returnUrl = "/")
        {
            Product? product = repository.Products
                .FirstOrDefault(p => p.ProductId == productId);

            if (product != null)
            {
                cart.AddItem(product, 1);
            }
            // Change this to redirect to Cart/Cart
            return RedirectToAction("Cart", "Cart");
        }

        [HttpPost]
        public IActionResult RemoveFromCart(int productId, string returnUrl = "/")
        {
            cart.RemoveLine(productId);
            return RedirectToAction("Cart", new { returnUrl });
        }
    }
}