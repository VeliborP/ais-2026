using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using WebShop.MVC.ViewModels.Cart;

namespace WebShop.MVC.ViewModels.Order
{
    public class CheckoutViewModel
    {
        [Required(ErrorMessage = "Full name is required")]
        [StringLength(150)]
        [DisplayName("Full name")]
        public string FullName { get; set; } = "";

        [Required(ErrorMessage = "Address is required")]
        [StringLength(250)]
        public string Address { get; set; } = "";

        [Required(ErrorMessage = "City is required")]
        [StringLength(100)]
        public string City { get; set; } = "";

        [Required(ErrorMessage = "Postal code is required")]
        [StringLength(20)]
        [DisplayName("Postal code")]
        public string PostalCode { get; set; } = "";

        [Required(ErrorMessage = "Phone is required")]
        [StringLength(30)]
        [Phone(ErrorMessage = "Phone number is not valid")]
        public string Phone { get; set; } = "";

        [StringLength(1000)]
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
