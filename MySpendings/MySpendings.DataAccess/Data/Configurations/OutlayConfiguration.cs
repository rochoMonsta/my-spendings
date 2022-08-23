using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MySpendings.Models;

namespace MySpendings.DataAccess.Data.Configurations
{
    public class OutlayConfiguration : IEntityTypeConfiguration<Outlay>
    {
        public void Configure(EntityTypeBuilder<Outlay> builder)
        {
            builder.HasKey(outlay => outlay.Id);
            builder.HasIndex(outlay => outlay.Id);
            builder.Property(outlay => outlay.Name).IsRequired().HasMaxLength(100);
            builder.Property(outlay => outlay.CategoryId).IsRequired();
            builder.Property(outlay => outlay.CreatedDate).IsRequired();
            builder.Property(outlay => outlay.Cost).IsRequired();
            builder.Property(outlay => outlay.Description).HasMaxLength(250);
        }
    }
}
