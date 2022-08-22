using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpendings.Models;

namespace MySpendings.DataAccess.Data.Configurations
{
    public class UserCategoryConfiguration : IEntityTypeConfiguration<UserCategory>
    {
        public void Configure(EntityTypeBuilder<UserCategory> builder)
        {
            builder.HasKey(userCategory => userCategory.Id);
            builder.HasIndex(userCategory => userCategory.Id);
            builder.Property(userCategory => userCategory.UserId).IsRequired();
            builder.Property(userCategory => userCategory.CategoryId).IsRequired();
        }
    }
}
