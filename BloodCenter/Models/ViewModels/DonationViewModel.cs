using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models.ViewModels
{
    public class DonationViewModel
    {
        [Required]
        public int BloodDonorId { get; set; }  // ID на кръводарителя

        [Required]
        public double Quantity { get; set; } 

        public DateTime DonationDate { get; set; } = DateTime.Now; 
    }
}
