using Microsoft.EntityFrameworkCore;
using MySpendings.DataAccess.Data.Configurations;
using MySpendings.Models;

namespace MySpendings.DataAccess.Data
{
    public class ApplicationDBContext : DbContext
    {
        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options) : base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Outlay> Outlays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new OutlayConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
