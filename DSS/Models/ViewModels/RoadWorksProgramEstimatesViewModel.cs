namespace DSS.Models.ViewModels
{
    public class RoadWorksProgramEstimatesViewModel
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string? Month { get; set; }
        public double? Cost { get; set; }
        public IEnumerable<Estimate>? Estimates { get; set; }
        public int RoadId { get; set; }
        public Road? Road { get; set; }
    }
}
