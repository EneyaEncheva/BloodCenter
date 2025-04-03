using BloodCenter.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class Requests
    {
        public int Id { get; set; }
        public string Hospital { get; set; } = "Университетска болница";

        [Required(ErrorMessage = "Това поле е задължително.")]
        public int BloodGroupId { get; set; }

        [ForeignKey("BloodGroupId")]
        public BloodGroups? BloodGroup { get; set; }

        [Required(ErrorMessage = "Това поле е задължително.")]
        public char RhesusFactor { get; set; }

        [Range(1, 10, ErrorMessage = "Количество трябва да бъде между 1 и 10 сака.")]

        [Required(ErrorMessage = "Това поле е задължително.")]
        public double Quantity { get; set; }
        public DateTime Date { get; set; } = DateTime.Now;
        public string Status { get; set; } = "В процес на изпълнение";
        public string? RequestedById { get; set; } 
        public ApplicationUser? RequestedBy { get; set; } 
        public DateTime? ExecutionDate { get; set; }
    }
}
