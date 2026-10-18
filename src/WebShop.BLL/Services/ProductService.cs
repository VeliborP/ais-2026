using Microsoft.EntityFrameworkCore;
using WebShop.BLL.Interfaces;
using WebShop.BLL.Models;
using WebShop.DAL;
using WebShop.DAL.Models;

namespace WebShop.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly WebShopDbContext _context;
        public ProductService(WebShopDbContext context)
        {
            _context = context;
        }

        public async Task<List<Product>> GetAllAsync()
        {
            return await _context.Products.Include(p => p.Category).ToListAsync();
        }

        public async Task<Product?> GetByIdAsync(int? id)
        {
            return await _context.Products.Include(p => p.Category).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product> CreateAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<Product> UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
                return false;

            var cartItems = await _context.CartItems.Where(c => c.ProductId == id).ToListAsync();
            _context.CartItems.RemoveRange(cartItems);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<Category>> GetCategoriesWithProductsAsync()
        {
            return await _context.Categories
            .Include(c => c.Products)
            .ToListAsync();
        }

        public async Task<PagedResult<Product>> SearchAsync(ProductFilter filter)
        {
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
            {
                var search = filter.Search.Trim();
                query = query.Where(p => p.Name.Contains(search)
                    || p.Code.Contains(search)
                    || (p.Description != null && p.Description.Contains(search)));
            }

            if (filter.CategoryId.HasValue)
                query = query.Where(p => p.CategoryId == filter.CategoryId.Value);

            if (filter.MinPrice.HasValue)
                query = query.Where(p => p.Price >= filter.MinPrice.Value);

            if (filter.MaxPrice.HasValue)
                query = query.Where(p => p.Price <= filter.MaxPrice.Value);

            query = filter.Sort switch
            {
                ProductSort.NameAsc => query.OrderBy(p => p.Name),
                ProductSort.NameDesc => query.OrderByDescending(p => p.Name),
                ProductSort.PriceAsc => query.OrderBy(p => p.Price),
                ProductSort.PriceDesc => query.OrderByDescending(p => p.Price),
                _ => query.OrderByDescending(p => p.Id)
            };

            var pageSize = filter.PageSize < 1 ? 12 : filter.PageSize;
            var totalCount = await query.CountAsync();
            var totalPages = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
            var page = Math.Clamp(filter.Page, 1, totalPages);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Product>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
