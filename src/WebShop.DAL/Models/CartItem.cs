using System.ComponentModel.DataAnnotations;

namespace WebShop.DAL.Models
{
    public class CartItem
    {
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        public User User { get; set; } = null!;

        [Required]
        public int ProductId { get; set; }

        public Product Product { get; set; } = null!;

        [Range(1, 1000)]
        public int Quantity { get; set; }

        public DateTime AddedAt { get; set; } = DateTime.Now;
    }
}
