using System.ComponentModel.DataAnnotations;

namespace MySpendings.Models.ViewModels
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Please provide Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Please provide Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
