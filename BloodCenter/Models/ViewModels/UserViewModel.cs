using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models.ViewModels
{
    public class UserViewModel
    {
        public string? Id { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Потребителско име")]
        public string UserName { get; set; }

        //[Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Парола")]
        [MinLength(6, ErrorMessage = "Паролата трябва да е поне 6 символа!")]
        public string? Password { get; set; }
        
        [MinLength(6, ErrorMessage = "Паролата трябва да е поне 6 символа!")]
        public string? NewPassword { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Електронна поща")]
        [EmailAddress(ErrorMessage = "Невалиден имейл адрес!")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Фамилия")]
        public string LastName { get; set; }

    }
}
