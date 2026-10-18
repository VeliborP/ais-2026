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
                TempData["Success"] = $"Status porudžbine je promenjen u „{OrderViewModel.GetStatusName(status)}“.";
            else
                TempData["Error"] = "Status porudžbine nije moguće promeniti.";

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
                TempData["Error"] = "Moguće je obrisati samo otkazane porudžbine.";
                return RedirectToAction(nameof(Details), new { id });
            }

            TempData["Success"] = "Porudžbina je obrisana.";
            return RedirectToAction(nameof(Index));
        }
    }
}
