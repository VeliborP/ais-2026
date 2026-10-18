using WebShop.DAL.Models;

namespace WebShop.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<Order?> CreateFromCartAsync(int userId, Order orderData);
        Task<List<Order>> GetByUserAsync(int userId);
        Task<Order?> GetForUserAsync(int id, int userId);
        Task<List<Order>> GetAllAsync(OrderStatus? status = null);
        Task<Order?> GetByIdAsync(int id);
        Task<bool> UpdateStatusAsync(int id, OrderStatus status);
        Task<bool> CancelByUserAsync(int id, int userId);
        Task<bool> DeleteAsync(int id);
        List<OrderStatus> GetAllowedNextStatuses(OrderStatus current);
    }
}
