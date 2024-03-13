namespace DSS.Models.ViewModels
{
    public class InputDataViewModel
    {
        public int InitialYear { get; set; }
        public string InitialMonth { get; set; }
        public int YearCount { get; set; }
        public double Budget { get; set; }
        public IEnumerable<string> Months { get; set; }
    }
}
