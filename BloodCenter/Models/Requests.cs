using BloodCenter.Data;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class Requests
    {
        public int Id { get; set; }
        public string Hospital { get; set; } = "Университетска болница";
        public int BloodGroupId { get; set; }

        [ForeignKey("BloodGroupId")]
        public BloodGroups? BloodGroup { get; set; }
        public char RhesusFactor { get; set; }
        public double Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "Изчаква одобрение";
        public string? RequestedById { get; set; } // ID на потребителя (медицинското лице)
        public ApplicationUser? RequestedBy { get; set; } // Навигационно свойство

    }
}
