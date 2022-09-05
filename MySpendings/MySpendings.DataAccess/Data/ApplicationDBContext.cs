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
        public DbSet<UserCategory> UserCategories { get; set; }
        public DbSet<UserOutlay> UserOutlays { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new OutlayConfiguration());
            modelBuilder.ApplyConfiguration(new UserCategoryConfiguration());
            modelBuilder.ApplyConfiguration(new UserOutlayConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
