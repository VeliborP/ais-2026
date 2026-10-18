using System.ComponentModel;
using WebShop.DAL.Models;

namespace WebShop.MVC.ViewModels.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [DisplayName("Order number")]
        public string OrderNumber { get; set; } = "";

        [DisplayName("Date")]
        public DateTime OrderDate { get; set; }

        public OrderStatus Status { get; set; }

        [DisplayName("Status changed")]
        public DateTime? StatusChangedAt { get; set; }

        [DisplayName("Customer")]
        public string Username { get; set; } = "";

        public string Email { get; set; } = "";

        [DisplayName("Full name")]
        public string FullName { get; set; } = "";

        public string Address { get; set; } = "";

        public string City { get; set; } = "";

        [DisplayName("Postal code")]
        public string PostalCode { get; set; } = "";

        public string Phone { get; set; } = "";

        public string? Note { get; set; }

        [DisplayName("Total")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Items")]
        public int ItemCount { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = [];

        public List<OrderStatus> AllowedStatuses { get; set; } = [];

        public bool CanCancel => Status == OrderStatus.New;

        public string StatusBadgeClass => GetStatusBadgeClass(Status);

        public static string GetStatusBadgeClass(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.New => "bg-primary",
                OrderStatus.Processing => "bg-info text-dark",
                OrderStatus.Shipped => "bg-warning text-dark",
                OrderStatus.Delivered => "bg-success",
                OrderStatus.Cancelled => "bg-secondary",
                _ => "bg-light text-dark"
            };
        }

        public static OrderViewModel FromEntity(DAL.Models.Order order, List<OrderStatus>? allowedStatuses = null)
        {
            return new OrderViewModel
            {
                Id = order.Id,
                OrderNumber = order.OrderNumber,
                OrderDate = order.OrderDate,
                Status = order.Status,
                StatusChangedAt = order.StatusChangedAt,
                Username = order.User?.Username ?? "",
                Email = order.User?.Email ?? "",
                FullName = order.FullName,
                Address = order.Address,
                City = order.City,
                PostalCode = order.PostalCode,
                Phone = order.Phone,
                Note = order.Note,
                TotalAmount = order.TotalAmount,
                ItemCount = order.Items.Sum(i => i.Quantity),
                Items = order.Items.Select(OrderItemViewModel.FromEntity).ToList(),
                AllowedStatuses = allowedStatuses ?? []
            };
        }
    }

    public class OrderItemViewModel
    {
        public int? ProductId { get; set; }
        public string ProductCode { get; set; } = "";
        public string ProductName { get; set; } = "";
        public decimal UnitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal LineTotal { get; set; }

        public static OrderItemViewModel FromEntity(OrderItem item)
        {
            return new OrderItemViewModel
            {
                ProductId = item.ProductId,
                ProductCode = item.ProductCode,
                ProductName = item.ProductName,
                UnitPrice = item.UnitPrice,
                Quantity = item.Quantity,
                LineTotal = item.LineTotal
            };
        }
    }
}
