using System.ComponentModel.DataAnnotations;

namespace MySpendings.Models.ViewModels
{
    public class RegisterModel
    {
        [Required(ErrorMessage = "Please provide Name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please provide Email")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please provide Login")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Please provide Income")]
        public int Income { get; set; }

        [Required(ErrorMessage = "Please provide Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Incorrect password ")]
        [Display(Name = "Confirm Password")]
        public string ConfirmPassword { get; set; }
    }
}
