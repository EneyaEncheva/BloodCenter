using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class Availability
    {
        public int Id { get; set; }
        public int BloodGroupId { get; set; }

        [ForeignKey("BloodGroupId")]
        public BloodGroups BloodGroup { get; set; }
        public char RhesusFactor { get; set; }
        public double Quantity { get; set; }
        public DateTime LastUpdated { get; set; }
    }
}
