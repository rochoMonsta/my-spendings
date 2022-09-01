using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MySpendings.Models.ViewModels
{
    public class CategoryStatusViewModel
    {
        [ValidateNever]
        public int CategoryId { get; set; }

        [ValidateNever]
        public string CategoryName { get; set; }

        [ValidateNever]
        public bool IsActive { get; set; }
    }
}
