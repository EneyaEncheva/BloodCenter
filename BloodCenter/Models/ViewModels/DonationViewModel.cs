using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models.ViewModels
{
    public class DonationViewModel
    {
        [Required(ErrorMessage = "Това поле е задължително.")]
        public int BloodDonorId { get; set; }

        [Range(405, 450, ErrorMessage = "Количеството трябва да е между 405 и 450 мл.")]
        [Required(ErrorMessage = "Това поле е задължително.")]
        public double Quantity { get; set; } 

        public DateTime DonationDate { get; set; } = DateTime.Now; 

    }
}
