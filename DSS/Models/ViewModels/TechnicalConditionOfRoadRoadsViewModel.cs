namespace DSS.Models.ViewModels
{
    public class TechnicalConditionOfRoadRoadsViewModel
    {
        public TechnicalConditionOfRoad TechnicalConditionOfRoad { get; set; }
        public IEnumerable<Road> Roads { get; set; }
        public IEnumerable<string> Months { get; set; }
    }
}
