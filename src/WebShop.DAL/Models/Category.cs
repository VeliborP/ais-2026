using System.ComponentModel.DataAnnotations;

namespace WebShop.DAL.Models;

public partial class Category : ISoftDelete
{
    public int Id { get; set; }
    [Required]
    [StringLength(50)]
    public required string Code { get; set; }
    [Required]
    [StringLength(150)]
    public required string Name { get; set; }
    [StringLength(1500)]
    public string? Description { get; set; }
    public ICollection<Product> Products { get; set; }
    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}
