using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpendings.Models;

namespace MySpendings.DataAccess.Data.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasKey(user => user.Id);
            builder.HasIndex(user => user.Id);
            builder.Property(user => user.Name).IsRequired().HasMaxLength(50);
            builder.Property(user => user.Login).IsRequired().HasMaxLength(50);
            builder.Property(user => user.Password).IsRequired().HasMaxLength(50);
            builder.Property(user => user.Email).IsRequired().HasMaxLength(50);
            builder.Property(user => user.Income).IsRequired();
        }
    }
}
