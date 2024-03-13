namespace DSS.Models.ViewModels
{
    public class RoadWorksProgramRoadsEstimatesViewModel
    {
        public RoadWorksProgram RoadWorksProgram { get; set; }
        public IEnumerable<Road> Roads { get; set; }
        public IEnumerable<Estimate> Estimates { get; set; }
        public IEnumerable<string> Months { get; set; }
    }
}
