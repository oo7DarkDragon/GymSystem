using GymSystem.Configurations;
using GymSystem.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Client;

namespace GymSystem.Contexts
{
    public class GYMDBContext:DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=.;database=GymDBG03;trusted_connection=true;trustServerCertificate=true");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfigurations());

            
        }

        public DbSet<Plan> Plans { get; set; }
    }
}
