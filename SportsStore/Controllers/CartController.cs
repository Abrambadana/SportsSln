using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;

namespace SportsStore.Controllers
{
    public class CartController : Controller
    {
        private readonly IStoreRepository repository;
        private readonly IOrderRepository orderRepository;
        private readonly Cart cart;

        public CartController(IStoreRepository repo, IOrderRepository orderRepo, Cart cartService)
        {
            repository = repo;
            orderRepository = orderRepo;
            cart = cartService;
        }

        public ViewResult Cart(string returnUrl = "/")
        {
            return View(new CartIndexViewModel { Cart = cart, ReturnUrl = returnUrl });
        }

        public IActionResult Completed()
        {
            // Get the order ID from TempData
            if (TempData["OrderId"] is int orderId)
            {
                // Retrieve the order from the database
                var order = orderRepository.Orders
                    .FirstOrDefault(o => o.OrderID == orderId);

                if (order != null)
                {
                    return View(order);  // Pass the order to the view
                }
            }

            // Redirect back to the Cart page if the order wasn't found
            return RedirectToAction("Cart", "Cart");
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