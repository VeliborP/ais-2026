using WebShop.DAL.Models;

namespace WebShop.BLL.Interfaces
{
    public interface ICartService
    {
        Task<List<CartItem>> GetItemsAsync(int userId);
        Task<int> GetItemCountAsync(int userId);
        Task<bool> AddAsync(int userId, int productId, int quantity = 1);
        Task<bool> UpdateQuantityAsync(int userId, int cartItemId, int quantity);
        Task<bool> RemoveAsync(int userId, int cartItemId);
        Task ClearAsync(int userId);
    }
}
