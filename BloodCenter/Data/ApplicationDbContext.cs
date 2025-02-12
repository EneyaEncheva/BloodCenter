using BloodCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BloodCenter.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<BloodDonors> BloodDonors { get; set; }
        public DbSet<BloodGroups> BloodGroups { get; set; }
        public DbSet<Availability> Availability { get; set; }
        public DbSet<Requests> Requests { get; set; }
    }
}
