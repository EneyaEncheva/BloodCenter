using System.ComponentModel.DataAnnotations;
using BloodCenter.Enums;

namespace BloodCenter.Models.ViewModels
{
    public class BloodDonorViewModel : UserViewModel
    {
        //??? Зa id, username........
        public int Id { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Имейл")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Фамилия")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Възраст")]
        public int Age { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Кръвна група")]
        public int BloodGroupsId { get; set; }

        public string? BloodGroupName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Резус-фактор")]
        public char RhesusFactor { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Контакти")]
        public string Contacts { get; set; }

    }
}
