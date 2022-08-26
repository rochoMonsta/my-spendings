using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MySpendings.Models.ViewModels
{
    public class CategoryStatusViewModel
    {
        [ValidateNever]
        public Category Category { get; set; }

        [ValidateNever]
        public bool IsActive { get; set; }
    }
}
