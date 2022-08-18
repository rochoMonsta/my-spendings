using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpendings.Models;

namespace MySpendings.DataAccess.Data.Configurations
{
    public class CategoryConfiguration : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            builder.HasKey(category => category.Id);
            builder.HasIndex(category => category.Id);
            builder.Property(category => category.Name).IsRequired().HasMaxLength(50);
            builder.Property(category => category.Description).HasMaxLength(250);
            builder.Property(category => category.Priority).IsRequired();
        }
    }
}
