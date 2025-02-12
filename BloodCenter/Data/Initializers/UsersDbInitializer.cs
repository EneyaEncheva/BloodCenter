using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;

namespace BloodCenter.Data.Initializers
{
    public static class UsersDbInitializer
    {
        //public static async void Seed(IApplicationBuilder applicationBuilder)
        //{
        //    using(var scope = applicationBuilder.ApplicationServices.CreateScope())
        //    {
        //        var roleManager = scope.ServiceProvider.GetService<RoleManager<IdentityRole>>();
        //        await CreateRoles(roleManager);
        //    }
        //}
        //public static async Task CreateRoles(RoleManager<IdentityRole> roleManager)
        //{
        //    if (!await roleManager.RoleExistsAsync("Admin"))
        //    {
        //        await roleManager.CreateAsync(new IdentityRole("Admin"));
        //        await roleManager.CreateAsync(new IdentityRole("Medical specialist"));
        //        await roleManager.CreateAsync(new IdentityRole("Donor"));
        //    }
        //}
        //public static async Task Initialize(IServiceProvider provider)
        //{
        //    // Добавяне на роли
        //    RoleManager<IdentityRole> roleManager =
        //        provider.GetRequiredService<RoleManager<IdentityRole>>();

        //    string adminRoleName = "Admin";

        //    if ((await roleManager.FindByNameAsync(adminRoleName)) == null)
        //    {
        //        await roleManager.CreateAsync(new IdentityRole(adminRoleName));
        //    }

        //    string doctorRoleName = "Medical specialist";

        //    if ((await roleManager.FindByNameAsync(doctorRoleName)) == null)
        //    {
        //        await roleManager.CreateAsync(new IdentityRole(doctorRoleName));
        //    }

        //    string bloodDonorRoleName = "Blood donor";

        //    if ((await roleManager.FindByNameAsync(bloodDonorRoleName)) == null)
        //    {
        //        await roleManager.CreateAsync(new IdentityRole(bloodDonorRoleName));
        //    }

        //    // Добавяне на потребители и задаване на роли за тях
        //    UserManager<IdentityUser> userManager =
        //        provider.GetRequiredService<UserManager<IdentityUser>>();

        //    IdentityUser foundUser = await userManager.FindByNameAsync("admin@gmail.com");

        //    if (foundUser == null)
        //    {
        //        ApplicationUser userEditor = new()
        //        {
        //            UserName = "admin@gmail.com",
        //            Email = "admin@gmail.com",
        //            FirstName = "Admin",
        //            LastName = "Admin",
        //            EmailConfirmed = true,
        //            PhoneNumberConfirmed = true
        //        };

        //        await userManager.CreateAsync(userEditor, "password");
        //        await userManager.AddToRoleAsync(userEditor, adminRoleName);
        //    }

        //    foundUser = await userManager.FindByNameAsync("doctor@europe.eu");

        //    if (foundUser == null)
        //    {
        //        IdentityUser userReader = new()
        //        {
        //            UserName = "reader@europe.eu",
        //            Email = "reader@europe.eu",
        //            EmailConfirmed = true,
        //            PhoneNumberConfirmed = true
        //        };

        //        await userManager.CreateAsync(userReader, "password");
        //        await userManager.AddToRoleAsync(userReader, doctorRoleName);
        //    }
        //}

    }
}
