using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class Requests
    {
        public int Id { get; set; }
        public string Hospital {  get; set; }
        public int BloodGroupsId { get; set; }

        [ForeignKey("BloodGroupId")]
        public BloodGroups BloodGroup { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; }
        public bool IsAvailable { get; set; }
    }
}
