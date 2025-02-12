using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models
{
    public class Availability
    {
        public int Id { get; set; }

        [Display(Name = "Кръвна група")]
        public int BloodDonorsId { get; set; }
        public BloodDonors? BloodDonors { get; set; }

        [Display(Name = "Количество")]
        public double Quantity { get; set; }

        [Display(Name = "Дата")]
        public DateTime Date { get; set; }
    }
}
