using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebShop.DAL.Models;

namespace WebShop.MVC.ViewModels
{
    public class CategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Šifra je obavezna")]
        [DisplayName("Šifra")]
        public required string Code { get; set; }

        [Required(ErrorMessage = "Naziv je obavezan")]
        [DisplayName("Naziv")]
        public required string Name { get; set; }

        [DisplayName("Opis")]
        public string? Description { get; set; }


        public static CategoryViewModel FromEntity(Category category)
        {
            return new CategoryViewModel
            {
                Id = category.Id,
                Code = category.Code,
                Name = category.Name,
                Description = category.Description ?? "---"
            };
        }

        public Category ToEntity()
        {
            return new Category
            {
                Id = this.Id,
                Code = this.Code.Trim(),
                Name = this.Name.Trim(),
                Description = this.Description?.Trim()
            };
        }
    }
}
