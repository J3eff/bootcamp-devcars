using DevCars.API.Persistence.Configurations;
using DevCars.API.Entities;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DevCars.API.Persistence
{
    public class DevCarsDbContext : DbContext
    {
        public DevCarsDbContext(DbContextOptions<DevCarsDbContext> options) : base(options)
        {
        }
        public DbSet<Car> Cars { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<ExtraOderItem> ExtraOderItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            //modelBuilder.ApplyConfiguration(new CarDbConfiguration());
            //modelBuilder.ApplyConfiguration(new CustomerDbConfiguration());
            //modelBuilder.ApplyConfiguration(new OrderDbConfiguration());
            //modelBuilder.ApplyConfiguration(new ExtraOderDbConfiguration());      
        }
    }
}
