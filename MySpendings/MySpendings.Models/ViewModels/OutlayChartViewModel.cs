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
        public List<Outlay> CurrentMonthOutlays { get; set; }

        [ValidateNever]
        public List<CategoryStatusViewModel> CategoryStatuses { get; set; }

        [ValidateNever]
        public int SelectedMonth { get; set; }

        [ValidateNever]
        public List<SelectListItem> MonthList { get; set; }

        [ValidateNever]
        public int SelectedYear { get; set; }

        [ValidateNever]
        public List<SelectListItem> YearsList { get; set; }

        [ValidateNever]
        public User User { get; set; }
    }
}
