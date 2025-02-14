using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models.ViewModels
{
    public class UserViewModel
    {
    //    [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string Id { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Парола")]
        public string? Password { get; set; }
        //[Required(ErrorMessage = "Това поле е задължително и трябва да е от поне 6 символа."), Display(Name = "Нова парола")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Имейл")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Фамилия")]
        public string LastName { get; set; }

    }
}
