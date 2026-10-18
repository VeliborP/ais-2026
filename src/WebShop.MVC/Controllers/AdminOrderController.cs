using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebShop.BLL.Interfaces;
using WebShop.DAL.Models;
using WebShop.MVC.ViewModels.Order;

namespace WebShop.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminOrderController : Controller
    {
        private readonly IOrderService _orderService;

        public AdminOrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        public async Task<IActionResult> Index(OrderStatus? status)
        {
            var orders = await _orderService.GetAllAsync(status);
            var vm = orders.Select(o => OrderViewModel.FromEntity(o)).ToList();

            ViewBag.SelectedStatus = status;
            return View(vm);
        }

        public async Task<IActionResult> Details(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            return View(OrderViewModel.FromEntity(order, _orderService.GetAllowedNextStatuses(order.Status)));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateStatus(int id, OrderStatus status)
        {
            if (await _orderService.UpdateStatusAsync(id, status))
                TempData["Success"] = $"Order status changed to {status}.";
            else
                TempData["Error"] = "Order status could not be changed.";

            return RedirectToAction(nameof(Details), new { id });
        }

        public async Task<IActionResult> Delete(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null) return NotFound();

            return View(OrderViewModel.FromEntity(order));
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!await _orderService.DeleteAsync(id))
            {
                TempData["Error"] = "Only cancelled orders can be deleted.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Order has been deleted.";
            return RedirectToAction(nameof(Index));
        }
    }
}
