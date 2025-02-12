using BloodCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace BloodCenter.Data.Initializers
{
    public class BloodGroupsDbInitializer
    {
        public static void Initialize(IServiceProvider serviceProvider) 
        {
            using ApplicationDbContext context = new(serviceProvider.GetRequiredService<DbContextOptions<ApplicationDbContext>>());
            if (context.BloodGroups.Any())
            {
                return;
            }

            context.BloodGroups.AddRange(
                new BloodGroups
                {
                    Name = "A"
                },
                new BloodGroups
                {
                    Name= "B"
                },
                new BloodGroups
                {
                    Name = "AB"
                },
                new BloodGroups
                {
                    Name = "0"
                }
                );
            context.SaveChanges();
        }
    }
}
