using Microsoft.EntityFrameworkCore;
using WebShop.BLL.Interfaces;
using WebShop.DAL;
using WebShop.DAL.Models;

namespace WebShop.BLL.Services
{
    public class OrderService : IOrderService
    {
        private static readonly Dictionary<OrderStatus, List<OrderStatus>> AllowedTransitions = new()
        {
            { OrderStatus.New, new List<OrderStatus> { OrderStatus.Processing, OrderStatus.Cancelled } },
            { OrderStatus.Processing, new List<OrderStatus> { OrderStatus.Shipped, OrderStatus.Cancelled } },
            { OrderStatus.Shipped, new List<OrderStatus> { OrderStatus.Delivered } },
            { OrderStatus.Delivered, new List<OrderStatus>() },
            { OrderStatus.Cancelled, new List<OrderStatus>() }
        };

        private readonly WebShopDbContext _context;

        public OrderService(WebShopDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> CreateFromCartAsync(int userId, Order orderData)
        {
            var cartItems = await _context.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();

            if (cartItems.Count == 0)
                return null;

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = userId,
                OrderDate = DateTime.Now,
                Status = OrderStatus.New,
                FullName = orderData.FullName.Trim(),
                Address = orderData.Address.Trim(),
                City = orderData.City.Trim(),
                PostalCode = orderData.PostalCode.Trim(),
                Phone = orderData.Phone.Trim(),
                Note = string.IsNullOrWhiteSpace(orderData.Note) ? null : orderData.Note.Trim()
            };

            foreach (var cartItem in cartItems)
            {
                order.Items.Add(new OrderItem
                {
                    ProductId = cartItem.ProductId,
                    ProductCode = cartItem.Product.Code,
                    ProductName = cartItem.Product.Name,
                    UnitPrice = cartItem.Product.Price,
                    Quantity = cartItem.Quantity,
                    LineTotal = cartItem.Product.Price * cartItem.Quantity
                });
            }

            order.TotalAmount = order.Items.Sum(i => i.LineTotal);

            _context.Orders.Add(order);
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();

            return order;
        }

        public async Task<List<Order>> GetByUserAsync(int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        public async Task<Order?> GetForUserAsync(int id, int userId)
        {
            return await _context.Orders
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
        }

        public async Task<List<Order>> GetAllAsync(OrderStatus? status = null)
        {
            var query = _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .AsQueryable();

            if (status.HasValue)
                query = query.Where(o => o.Status == status.Value);

            return await query.OrderByDescending(o => o.OrderDate).ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<bool> UpdateStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || !GetAllowedNextStatuses(order.Status).Contains(status))
                return false;

            order.Status = status;
            order.StatusChangedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelByUserAsync(int id, int userId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == id && o.UserId == userId);
            if (order == null || order.Status != OrderStatus.New)
                return false;

            order.Status = OrderStatus.Cancelled;
            order.StatusChangedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null || order.Status != OrderStatus.Cancelled)
                return false;

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();
            return true;
        }

        public List<OrderStatus> GetAllowedNextStatuses(OrderStatus current)
        {
            return AllowedTransitions.TryGetValue(current, out var statuses)
                ? new List<OrderStatus>(statuses)
                : new List<OrderStatus>();
        }

        private static string GenerateOrderNumber()
        {
            return $"ORD-{DateTime.Now:yyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpper()}";
        }
    }
}
