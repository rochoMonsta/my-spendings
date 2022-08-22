using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MySpendings.Models.ViewModels
{
    public class OutlayViewModel
    {
        public Outlay Outlay { get; set; }

        [ValidateNever]
        public string MinDate { get; set; }

        [ValidateNever]
        public string MaxDate { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> Categories { get; set; }
    }
}
