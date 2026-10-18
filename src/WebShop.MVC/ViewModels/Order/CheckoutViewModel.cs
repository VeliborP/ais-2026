using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebShop.MVC.ViewModels.Cart;

namespace WebShop.MVC.ViewModels.Order
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Ime i prezime je obavezno")]
        [StringLength(150, ErrorMessage = "Maksimalno 150 karaktera")]
        [DisplayName("Ime i prezime")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Adresa je obavezna")]
        [StringLength(250, ErrorMessage = "Maksimalno 250 karaktera")]
        [DisplayName("Adresa")]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "Grad je obavezan")]
        [StringLength(100, ErrorMessage = "Maksimalno 100 karaktera")]
        [DisplayName("Grad")]
        public string City { get; set; } = "";

        [Required(ErrorMessage = "Poštanski broj je obavezan")]
        [StringLength(20, ErrorMessage = "Maksimalno 20 karaktera")]
        [DisplayName("Poštanski broj")]
        public string PostalCode { get; set; } = "";

        [Required(ErrorMessage = "Telefon je obavezan")]
        [StringLength(30, ErrorMessage = "Maksimalno 30 karaktera")]
        [Phone(ErrorMessage = "Broj telefona nije ispravan")]
        [DisplayName("Telefon")]
        public string Phone { get; set; } = "";

        [StringLength(1000, ErrorMessage = "Maksimalno 1000 karaktera")]
        [DisplayName("Napomena")]
        public string? Note { get; set; }

        public CartViewModel Cart { get; set; } = new CartViewModel();

        public DAL.Models.Order ToEntity()
        {
            return new DAL.Models.Order
            {
                FullName = FullName,
                Address = Address,
                City = City,
                PostalCode = PostalCode,
                Phone = Phone,
                Note = Note
            };
        }
    }
}
