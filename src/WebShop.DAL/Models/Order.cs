using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.DAL.Models
{
    public class Order
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string OrderNumber { get; set; } = "";

        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        public DateTime OrderDate { get; set; } = DateTime.Now;

        public OrderStatus Status { get; set; } = OrderStatus.New;

        [Required]
        [StringLength(150)]
        public string FullName { get; set; } = "";

        [Required]
        [StringLength(250)]
        public string Address { get; set; } = "";

        [Required]
        [StringLength(100)]
        public string City { get; set; } = "";

        [Required]
        [StringLength(20)]
        public string PostalCode { get; set; } = "";

        [Required]
        [StringLength(30)]
        public string Phone { get; set; } = "";

        [StringLength(1000)]
        public string? Note { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        public DateTime? StatusChangedAt { get; set; }

        public ICollection<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}
