using Microsoft.AspNetCore.Identity;
using BloodCenter.Models;

namespace BloodCenter.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public ICollection<BloodDonors> BloodDonors { get; set; }
        
    }
}
