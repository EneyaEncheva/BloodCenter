using System.ComponentModel.DataAnnotations;

namespace BloodCenter.Models.ViewModels
{
    public class UserViewModel
    {
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string Id { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Това поле е задължително."), Display(Name = "Име")]
        public string LastName { get; set; }

    }
}
