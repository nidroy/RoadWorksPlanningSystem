namespace DSS.Models.ViewModels
{
    public class RoadWorksProgramViewModel
    {
        public int Year { get; set; }
        public string? Month { get; set; }
        public double? Cost { get; set; }
        public List<int>? EstimatesId { get; set; }
        public int RoadId { get; set; }
    }
}
