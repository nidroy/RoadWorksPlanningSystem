namespace DSS.Models.ViewModels
{
    public class RoadRoadWorksProgramsViewModel
    {
        public Road Road { get; set; }
        public Dictionary<int, IEnumerable<RoadWorksProgramEstimatesViewModel>> RoadWorksPrograms { get; set; }
    }
}
