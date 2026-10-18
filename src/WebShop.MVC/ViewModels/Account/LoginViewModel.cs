using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace WebShop.MVC.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Korisničko ime je obavezno")]
        [DisplayName("Korisničko ime")]
        public string Username { get; set; } = "";

        [Required(ErrorMessage = "Lozinka je obavezna")]
        [DisplayName("Lozinka")]
        public string Password { get; set; } = "";
    }
}
