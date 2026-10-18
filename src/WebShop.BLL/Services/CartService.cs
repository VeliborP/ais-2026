using Microsoft.EntityFrameworkCore;
using WebShop.BLL.Interfaces;
using WebShop.DAL;
using WebShop.DAL.Models;

namespace WebShop.BLL.Services
{
    public class CartService : ICartService
    {
        private const int MaxQuantity = 100;

        private readonly WebShopDbContext _context;

        public CartService(WebShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<CartItem>> GetItemsAsync(int userId)
        {
            return await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .OrderBy(c => c.AddedAt)
                .ToListAsync();
        }

        public async Task<int> GetItemCountAsync(int userId)
        {
            return await _context.CartItems
                .Where(c => c.UserId == userId)
                .SumAsync(c => c.Quantity);
        }

        public async Task<bool> AddAsync(int userId, int productId, int quantity = 1)
        {
            if (quantity < 1)
                return false;

            var productExists = await _context.Products.AnyAsync(p => p.Id == productId);
            if (!productExists)
                return false;

            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == productId);

            if (item == null)
            {
                _context.CartItems.Add(new CartItem
                {
                    UserId = userId,
                    ProductId = productId,
                    Quantity = Math.Min(quantity, MaxQuantity)
                });
            }
            else
            {
                item.Quantity = Math.Min(item.Quantity + quantity, MaxQuantity);
            }

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateQuantityAsync(int userId, int cartItemId, int quantity)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
            if (item == null)
                return false;

            if (quantity < 1)
                _context.CartItems.Remove(item);
            else
                item.Quantity = Math.Min(quantity, MaxQuantity);

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RemoveAsync(int userId, int cartItemId)
        {
            var item = await _context.CartItems
                .FirstOrDefaultAsync(c => c.Id == cartItemId && c.UserId == userId);
            if (item == null)
                return false;

            _context.CartItems.Remove(item);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task ClearAsync(int userId)
        {
            var items = await _context.CartItems.Where(c => c.UserId == userId).ToListAsync();
            _context.CartItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }
    }
}
