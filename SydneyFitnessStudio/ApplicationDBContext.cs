using Microsoft.EntityFrameworkCore;
using SydneyFitnessStudio.Models;

namespace SydneyFitnessStudio
{
    public class FitnessStudioContext : DbContext
    {
        public FitnessStudioContext(DbContextOptions<FitnessStudioContext> options)
            : base(options)
        {
        }

        public DbSet<Person> People { get; set; }
        public DbSet<FitnessClass> FitnessClasses { get; set; }
        public DbSet<Reservation> Reservations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // You can define specific entity configurations here, such as relationships, keys, etc.
            base.OnModelCreating(modelBuilder);
        }
    }
}
