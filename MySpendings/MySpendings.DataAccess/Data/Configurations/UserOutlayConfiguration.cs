using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpendings.Models;

namespace MySpendings.DataAccess.Data.Configurations
{
    public class UserOutlayConfiguration : IEntityTypeConfiguration<UserOutlay>
    {
        public void Configure(EntityTypeBuilder<UserOutlay> builder)
        {
            builder.HasKey(userOutlay => userOutlay.Id);
            builder.HasIndex(userOutlay => userOutlay.Id);
            builder.Property(userOutlay => userOutlay.UserId).IsRequired();
            builder.Property(userOutlay => userOutlay.OutlayId).IsRequired();
        }
    }
}
