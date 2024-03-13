using System.ComponentModel.DataAnnotations.Schema;

namespace DSS.Models
{
    public class Estimate
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public double? LevelOfWorks { get; set; }
        public double? Cost { get; set; }
        public string? Link { get; set; }

        [ForeignKey("Road")]
        public int RoadId { get; set; }
        public Road? Road { get; set; }
    }
}
