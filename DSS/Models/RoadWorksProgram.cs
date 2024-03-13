using System.ComponentModel.DataAnnotations.Schema;

namespace DSS.Models
{
    public class RoadWorksProgram
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string? Month { get; set; }
        public double? Cost { get; set; }

        [ForeignKey("Road")]
        public int RoadId { get; set; }
        public Road? Road { get; set; }
    }
}
