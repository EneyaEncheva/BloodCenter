using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class DonationHistory
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Това поле е задължително.")]
        public int BloodDonorId { get; set; }

        [ForeignKey("BloodDonorId")]
        public BloodDonors BloodDonor { get; set; }
        public DateTime DonationDate { get; set; }

        [Required(ErrorMessage = "Това поле е задължително.")]
        public double Quantity { get; set; }
    }
}
