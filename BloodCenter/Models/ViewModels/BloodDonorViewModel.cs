using System.ComponentModel.DataAnnotations;
using BloodCenter.Enums;

namespace BloodCenter.Models.ViewModels
{
    public class BloodDonorViewModel : UserViewModel
    {
        public int Id { get; set; }

        [Range(18, 65, ErrorMessage = "Възрастта трябва да е между 18 и 65 години.")]
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Възраст")]

        public int Age { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Кръвна група")]
        public int BloodGroupsId { get; set; }

        public string? BloodGroupName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Резус-фактор")]
        public char RhesusFactor { get; set; }

        [RegularExpression(@"^\d{10}$", ErrorMessage = "Моля, въведете телефонен номер с точно 10 цифри.")]
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Контакти")]
        public string Contacts { get; set; }

        public int CurrentPage {  get; set; }
        public int TotalPages { get; set; }
    }
}
