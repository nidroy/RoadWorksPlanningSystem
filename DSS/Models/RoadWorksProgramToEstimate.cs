using System.ComponentModel.DataAnnotations.Schema;

namespace DSS.Models
{
    public class RoadWorksProgramToEstimate
    {
        public int Id { get; set; }

        [ForeignKey("RoadWorksProgram")]
        public int RoadWorksProgramId { get; set; }
        public RoadWorksProgram? RoadWorksProgram { get; set; }

        [ForeignKey("Estimate")]
        public int EstimateId { get; set; }
        public Estimate? Estimate { get; set; }
    }
}
