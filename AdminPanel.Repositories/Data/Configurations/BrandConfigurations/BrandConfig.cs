using AdminPanel.Core.Entities.Brands;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPanel.Repositories.Data.Configurations.BrandConfigurations
{
    public class BrandConfig : IEntityTypeConfiguration<Brand>
    {
        public void Configure(EntityTypeBuilder<Brand> builder)
        {
            builder.Property(B => B.CreatedAt).HasColumnType("datetime2(0)");
            builder.Property(B => B.LastModifiedAt).HasColumnType("datetime2(0)");
        }
    }
}
