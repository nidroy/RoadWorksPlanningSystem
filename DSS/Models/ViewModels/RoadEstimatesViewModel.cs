namespace DSS.Models.ViewModels
{
    public class RoadEstimatesViewModel
    {
        public Road Road { get; set; }
        public IEnumerable<Estimate> Estimates { get; set; }
    }
}
