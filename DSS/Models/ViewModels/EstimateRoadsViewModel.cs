namespace DSS.Models.ViewModels
{
    public class EstimateRoadsViewModel
    {
        public Estimate Estimate { get; set; }
        public IEnumerable<Road> Roads { get; set; }
    }
}
