using GymSystem.DAL.Configurations;
using GymSystem.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Context
{
    public class GymDbContext:DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=.;database=GymDBG03;trusted_connection=true;trustServerCertificate=true");
        //}

       public GymDbContext(DbContextOptions<GymDbContext> options) : base(options)
        {

        }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration<Booking>(new BookingConfigurations());
            modelBuilder.ApplyConfiguration<Category>(new CategoryConfigurations());
            modelBuilder.ApplyConfiguration<HealthRecord>(new HealthRecordConfigurations());
            modelBuilder.ApplyConfiguration<Member>(new MemberConfigurations());
            modelBuilder.ApplyConfiguration<Membership>(new MembershipConfigurations());
            modelBuilder.ApplyConfiguration<Session>(new SessionConfigurations());
            modelBuilder.ApplyConfiguration<Plan>(new PlanConfigurations());
            modelBuilder.ApplyConfiguration<Trainer>(new TrainerConfigurations());
            


        }

        public DbSet<Plan> Plans { get; set; }
    }
}
