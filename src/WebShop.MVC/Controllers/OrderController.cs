using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.BLL.Interfaces;
using WebShop.MVC.Extensions;
using WebShop.MVC.ViewModels.Cart;
using WebShop.MVC.ViewModels.Order;

namespace WebShop.MVC.Controllers
{
    [Authorize(Roles = "User")]
    public class OrderController : Controller
    {
        private readonly IOrderService _orderService;
        private readonly ICartService _cartService;

        public OrderController(IOrderService orderService, ICartService cartService)
        {
            _orderService = orderService;
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var orders = await _orderService.GetByUserAsync(User.GetUserId());
            var vm = orders.Select(o => OrderViewModel.FromEntity(o)).ToList();
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetForUserAsync(id, User.GetUserId());
            if (order == null) return NotFound();

            return View(OrderViewModel.FromEntity(order));
        }

        public async Task<IActionResult> Checkout()
        {
            var cart = await LoadCartAsync();
            if (cart.IsEmpty)
            {
                TempData["Error"] = "Vaša korpa je prazna.";
                return RedirectToAction("Index", "Cart");
            }

            return View(new CheckoutViewModel { Cart = cart });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                vm.Cart = await LoadCartAsync();
                return View(vm);
            }

            var order = await _orderService.CreateFromCartAsync(User.GetUserId(), vm.ToEntity());
            if (order == null)
            {
                TempData["Error"] = "Vaša korpa je prazna.";
                return RedirectToAction("Index", "Cart");
            }

            TempData["Success"] = $"Porudžbina {order.OrderNumber} je uspešno kreirana.";
            return RedirectToAction(nameof(Details), new { id = order.Id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (await _orderService.CancelByUserAsync(id, User.GetUserId()))
                TempData["Success"] = "Porudžbina je otkazana.";
            else
                TempData["Error"] = "Moguće je otkazati samo nove porudžbine.";

            return RedirectToAction(nameof(Details), new { id });
        }

        private async Task<CartViewModel> LoadCartAsync()
        {
            var items = await _cartService.GetItemsAsync(User.GetUserId());
            return CartViewModel.FromEntities(items);
        }
    }
}
