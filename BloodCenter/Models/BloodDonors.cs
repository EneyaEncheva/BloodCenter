using BloodCenter.Data;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCenter.Models
{
    public class BloodDonors
    {
        public int Id { get; set; }

        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        [Display(Name = "Възраст")]
        public int Age { get; set; }
        public int BloodGroupId { get; set; }

        [Display(Name = "Кръвна група")]
        public BloodGroups? BloodGroup { get; set; }

        [Display(Name = "Резус-фактор")]
        public char RhesusFactor { get; set; }

        [Display(Name = "Контактна информация")]
        public string Contacts { get; set; }
        public ICollection<DonationHistory> DonationHistory { get; set; } = 
            new List<DonationHistory>();
    }
}
