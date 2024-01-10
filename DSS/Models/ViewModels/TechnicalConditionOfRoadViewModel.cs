namespace DSS.Models.ViewModels
{
    public class TechnicalConditionOfRoadViewModel
    {
        public int Year { get; set; }
        public string? Month { get; set; }
        public double TechnicalCondition { get; set; }
        public int RoadId { get; set; }
    }
}
