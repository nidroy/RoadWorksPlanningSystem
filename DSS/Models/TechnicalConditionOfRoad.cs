using System.ComponentModel.DataAnnotations.Schema;

namespace DSS.Models
{
    public class TechnicalConditionOfRoad
    {
        public int Id { get; set; }
        public int Year { get; set; }
        public string? Month { get; set; }
        public double TechnicalCondition { get; set; }

        [ForeignKey("Road")]
        public int RoadId { get; set; }
        public Road? Road { get; set; }
    }
}
