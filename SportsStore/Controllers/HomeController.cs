using Microsoft.AspNetCore.Mvc;
using SportsStore.Models;
using SportsStore.Models.ViewModels;
using System.Linq;

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
                    TotalItems = category == null
                        ? storeRepository.Products.Count()
                        : storeRepository.Products.Where(e => e.Category == category).Count()
                },
                CurrentCategory = category
            });
        }

        public IActionResult Checkout()
        {
            if (!cart.Lines.Any())
            {
                ModelState.AddModelError("", "Your cart is empty!");
                return RedirectToAction("Cart", "Cart");
            }

            return View(new Order());
        }

        [HttpPost]
        public IActionResult Checkout(Order order)
        {
            if (!cart.Lines.Any())
            {
                ModelState.AddModelError("", "Your cart is empty!");
                return View(order);
            }

            order.Lines = cart.Lines.Select(line => new CartLine
            {
                ProductId = line.Product.ProductId ?? 0,
                Product = line.Product,
                Quantity = line.Quantity
            }).ToList();

            if (ModelState.IsValid)
            {
                int orderId = orderRepository.SaveOrder(order);
                cart.Clear();
                TempData["OrderId"] = orderId;
                return RedirectToAction("Completed", "Cart");
            }

            return View(order);
        }

        public IActionResult Completed()
        {
            return RedirectToAction("Completed", "Cart");
        }
    }
}
