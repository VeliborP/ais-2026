using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.BLL.Interfaces;
using WebShop.MVC.Extensions;
using WebShop.MVC.ViewModels.Cart;

namespace WebShop.MVC.Controllers
{
    [Authorize(Roles = "User")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        public async Task<IActionResult> Index()
        {
            var items = await _cartService.GetItemsAsync(User.GetUserId());
            return View(CartViewModel.FromEntities(items));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(int productId, int quantity = 1, string? returnUrl = null)
        {
            var added = await _cartService.AddAsync(User.GetUserId(), productId, quantity);

            if (added)
                TempData["Success"] = "Proizvod je dodat u korpu.";
            else
                TempData["Error"] = "Proizvod nije moguće dodati u korpu.";

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(int id, int quantity)
        {
            if (!await _cartService.UpdateQuantityAsync(User.GetUserId(), id, quantity))
                TempData["Error"] = "Stavka korpe nije pronađena.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Remove(int id)
        {
            if (!await _cartService.RemoveAsync(User.GetUserId(), id))
                TempData["Error"] = "Stavka korpe nije pronađena.";

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Clear()
        {
            await _cartService.ClearAsync(User.GetUserId());
            return RedirectToAction(nameof(Index));
        }
    }
}
