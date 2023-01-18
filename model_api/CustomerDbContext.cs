using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using model_api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace model_api
{
    public class CustomerDbContext:DbContext
    {
       
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
         
            modelBuilder.Entity<Customer>().ToTable("Customer");
        }
        //protected override void OnModelCreating(ModelBuilder builder)
        //{
        //    builder.Entity<Customer>()
        //        .HasIndex(u => u.Email)
        //        .IsUnique();
        //}
        public  CustomerDbContext(DbContextOptions<CustomerDbContext> dbcontextoption): base(dbcontextoption)
        {
        
            try
            {
                RelationalDatabaseCreator databasecreator = Database.GetService<IDatabaseCreator>() as RelationalDatabaseCreator;
                if (databasecreator != null)
                {
                    if (!databasecreator.CanConnect())
                    {
                      //  databasecreator.EnsureDeletedAsync();
                        databasecreator.EnsureCreatedAsync();
                    }
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }


        public DbSet<Customer> Customers { get; set; }
        public DbSet<Payment_info> Pyament_infos { get; set; }
        public DbSet<AuthCode> AuthCodes { get; set; }
        public DbSet<CustomerLogs> Customerlogs { get; set; }
        public DbSet<Admin>  Admins  { get; set; }
        
    }
}
