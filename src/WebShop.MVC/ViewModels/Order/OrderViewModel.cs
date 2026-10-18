using System.ComponentModel;
using WebShop.DAL.Models;

namespace WebShop.MVC.ViewModels.Order
{
    public class OrderViewModel
    {
        public int Id { get; set; }

        [DisplayName("Broj porudžbine")]
        public string OrderNumber { get; set; } = "";

        [DisplayName("Datum")]
        public DateTime OrderDate { get; set; }

        [DisplayName("Status")]
        public OrderStatus Status { get; set; }

        [DisplayName("Status promenjen")]
        public DateTime? StatusChangedAt { get; set; }

        [DisplayName("Kupac")]
        public string Username { get; set; } = "";

        [DisplayName("Email")]
        public string Email { get; set; } = "";

        [DisplayName("Ime i prezime")]
        public string FullName { get; set; } = "";

        [DisplayName("Adresa")]
        public string Address { get; set; } = "";

        [DisplayName("Grad")]
        public string City { get; set; } = "";

        [DisplayName("Poštanski broj")]
        public string PostalCode { get; set; } = "";

        [DisplayName("Telefon")]
        public string Phone { get; set; } = "";

        [DisplayName("Napomena")]
        public string? Note { get; set; }

        [DisplayName("Ukupno")]
        public decimal TotalAmount { get; set; }

        [DisplayName("Broj artikala")]
        public int ItemCount { get; set; }

        public List<OrderItemViewModel> Items { get; set; } = [];

        public List<OrderStatus> AllowedStatuses { get; set; } = [];

        public bool CanCancel => Status == OrderStatus.New;

        public string StatusBadgeClass => GetStatusBadgeClass(Status);

        public string StatusName => GetStatusName(Status);

        public static string GetStatusName(OrderStatus status)
        {
            return status switch
            {
                OrderStatus.New => "Nova",
                OrderStatus.Processing => "U obradi",
                OrderStatus.Shipped => "Poslata",
                OrderStatus.Delivered => "Isporučena",
                OrderStatus.Cancelled => "Otkazana",
                _ => status.ToString()
            };
        }

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
