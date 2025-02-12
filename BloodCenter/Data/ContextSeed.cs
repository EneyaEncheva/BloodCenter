using BloodCenter.Enums;
using Microsoft.AspNetCore.Identity;

namespace BloodCenter.Data
{
    public class ContextSeed
    {
        public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
        {
            foreach (string roleName in Enum.GetNames(typeof(Role)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
        }

        public static async Task SeedAdminAsync(UserManager<ApplicationUser> userManager)
        {
            ApplicationUser defaultUser = new()
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            ApplicationUser foundUser = await userManager.FindByEmailAsync(defaultUser.Email);

            if (foundUser == null)
            {
                await userManager.CreateAsync(defaultUser, "abcdef");

                await userManager.AddToRoleAsync(defaultUser, Role.Donor.ToString());
                await userManager.AddToRoleAsync(defaultUser, Role.MedicalSpecialist.ToString());
                await userManager.AddToRoleAsync(defaultUser, Role.Admin.ToString());
                
            }
        }
    }
}
