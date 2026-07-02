using GymSystem.DAL.Configurations;
using GymSystem.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GymSystem.DAL.Context
{
    public class GymDbContext:IdentityDbContext<ApplicationUser>
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

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            //modelBuilder.ApplyConfiguration<Booking>(new BookingConfigurations());
            //modelBuilder.ApplyConfiguration<Category>(new CategoryConfigurations());
            //modelBuilder.ApplyConfiguration<HealthRecord>(new HealthRecordConfigurations());
            //modelBuilder.ApplyConfiguration<Member>(new MemberConfigurations());
            //modelBuilder.ApplyConfiguration<Membership>(new MembershipConfigurations());
            //modelBuilder.ApplyConfiguration<Session>(new SessionConfigurations());
            //modelBuilder.ApplyConfiguration<Plan>(new PlanConfigurations());
            //modelBuilder.ApplyConfiguration<Trainer>(new TrainerConfigurations());
            


        }

        public DbSet<Plan> Plans { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Membership> Memberships { get; set; }
        public DbSet<Member> Members { get; set; }
        public DbSet<HealthRecord> HealthRecords { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Booking> Bookings { get; set; }


    }
}
