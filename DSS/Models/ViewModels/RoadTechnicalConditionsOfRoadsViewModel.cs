namespace DSS.Models.ViewModels
{
    public class RoadTechnicalConditionsOfRoadsViewModel
    {
        public Road Road { get; set; }
        public Dictionary<int, IEnumerable<TechnicalConditionOfRoad>> TechnicalConditionsOfRoads { get; set; }
    }
}
