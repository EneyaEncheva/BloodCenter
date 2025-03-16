using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models.ViewModels
{
    public class DonationViewModel
    {
        [Required(ErrorMessage = "Това поле е задължително.")]
        public int BloodDonorId { get; set; }  // ID на кръводарителя

        [Required(ErrorMessage = "Това поле е задължително.")]
        public double Quantity { get; set; } 

        public DateTime DonationDate { get; set; } = DateTime.Now; 
    }
}
