using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization; // Add this for ReferenceHandler

namespace SportsStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly IStoreRepository storeRepository;
        private readonly IOrderRepository orderRepository;
        private readonly Cart cart;
        public int PageSize = 4;

        public HomeController(IStoreRepository storeRepo, IOrderRepository orderRepo, Cart cartService)
        {
            storeRepository = storeRepo;
            orderRepository = orderRepo;
            cart = cartService;
        }

        public ViewResult Index(string category, int productPage = 1)
        {
            return View(new ProductsListViewModel
            {
                Products = storeRepository.Products
                    .Where(p => category == null || p.Category == category)
                    .OrderBy(p => p.ProductId)
                    .Skip((productPage - 1) * PageSize)
                    .Take(PageSize),
                PagingInfo = new PagingInfo
                {
                    CurrentPage = productPage,
                    ItemsPerPage = PageSize,
                    TotalItems = category == null ?
                        storeRepository.Products.Count() :
                        storeRepository.Products.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            });
        }

        public IActionResult Checkout()
        {
            // Check if cart is empty
            if (cart.Lines.Count() == 0)
            {
                ModelState.AddModelError("", "Sorry, your cart is empty!");
                return RedirectToAction("Cart", "Cart");
            }
            return View(new Order());
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (cart.Lines.Count == 0)
            {
                ModelState.AddModelError("", "Your cart is empty!");
                return View(order);
            }

            // Populate the order's Lines collection from the cart
            // IMPORTANT: Do this BEFORE checking ModelState.IsValid
            order.Lines = cart.Lines.Select(line => new CartLine
            {
                ProductId = line.Product.ProductId ?? 0,
                Product = line.Product,
                Quantity = line.Quantity
            }).ToList();

            if (ModelState.IsValid)
            {
                try
                {
                    // Save the order
                    int orderId = orderRepository.SaveOrder(order);

                    // Clear the cart
                    cart.Clear();

                    // Instead of serializing the entire order, just store the order ID
                    TempData["OrderId"] = orderId;

                    // Redirect to the Completed action in CartController
                    return RedirectToAction("Completed", "Cart");
                }
                catch (Exception ex)
                {
                    // Log the exception
                    ModelState.AddModelError("", "Error saving order: " + ex.Message);
                }
            }

            // If we got here, something went wrong
            return View(order);
        }

        // Add a Completed action as fallback
        public IActionResult Completed()
        {
            // Redirect to Cart controller's Completed action
            return RedirectToAction("Completed", "Cart");
        }
    }
}