namespace MySpendings.Models.ViewModels
{
    public class OutlayChartViewModel
    {
        public Dictionary<string, float> CategoryOutlays { get; set; }
        public string OutlaysData { get; set; }
        public IEnumerable<Outlay> Outlays { get; set; }
    }
}
