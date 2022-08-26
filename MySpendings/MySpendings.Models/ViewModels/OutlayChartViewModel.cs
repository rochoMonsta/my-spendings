using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MySpendings.Models.ViewModels
{
    public class OutlayChartViewModel
    {
        [ValidateNever]
        public Dictionary<string, float> CurrentMonthCategoryOutlays { get; set; }

        [ValidateNever]
        public string OutlaysData { get; set; }

        [ValidateNever]
        public IEnumerable<Outlay> CurrentMonthOutlays { get; set; }

        [ValidateNever]
        public int SelectedMonth { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> MonthList { get; set; }

        [ValidateNever]
        public int SelectedYear { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> YearsList { get; set; }
    }
}
