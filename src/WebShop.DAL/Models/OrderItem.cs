using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebShop.DAL.Models
{
    public class OrderItem
    {
        public int Id { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; } = null!;

        public int? ProductId { get; set; }

        public Product? Product { get; set; }

        [Required]
        [StringLength(50)]
        public string ProductCode { get; set; } = "";

        [Required]
        [StringLength(150)]
        public string ProductName { get; set; } = "";

        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Range(1, 1000)]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal LineTotal { get; set; }
    }
}
