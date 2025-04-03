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
            ApplicationUser admin = new()
            {
                UserName = "admin",
                Email = "admin@gmail.com",
                FirstName = "Admin",
                LastName = "Admin",
                EmailConfirmed = true,
                PhoneNumberConfirmed = true
            };

            ApplicationUser foundUser = await userManager.FindByEmailAsync(admin.Email);

            if (foundUser == null)
            {
                await userManager.CreateAsync(admin, "abcdef");
                await userManager.AddToRoleAsync(admin, Role.Admin.ToString());
            }
        }
    }
}
