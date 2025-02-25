using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class DonationHistory
    {
        public int Id { get; set; }
        public int BloodDonorId { get; set; }

        [ForeignKey("BloodDonorId")]
        public BloodDonors BloodDonor { get; set; }

        public DateTime DonationDate { get; set; }
        public double Quantity { get; set; }
    }
}
