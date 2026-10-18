using WebShop.DAL.Models;

namespace WebShop.MVC.ViewModels.Cart
{
    public class CartViewModel
    {
        public List<CartItemViewModel> Items { get; set; } = [];

        public int TotalQuantity => Items.Sum(i => i.Quantity);

        public decimal TotalAmount => Items.Sum(i => i.LineTotal);

        public bool IsEmpty => Items.Count == 0;

        public static CartViewModel FromEntities(IEnumerable<CartItem> items)
        {
            return new CartViewModel
            {
                Items = items.Select(CartItemViewModel.FromEntity).ToList()
            };
        }
    }

    public class CartItemViewModel
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public string? ImagePath { get; set; }
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal => UnitPrice * Quantity;

        public static CartItemViewModel FromEntity(CartItem item)
        {
            return new CartItemViewModel
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductCode = item.Product.Code,
                ProductName = item.Product.Name,
                ImagePath = item.Product.ImagePath,
                UnitPrice = item.Product.Price,
                Quantity = item.Quantity
            };
        }
    }
}
