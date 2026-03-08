using AdminPanel.Core.Entities.Products;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Repositories.Data.Configurations.ProductConfigurations
{
    public class ProductConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.Property(P => P.Price).HasColumnType("decimal(18, 2)");
            builder.Property(P => P.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

            builder.HasOne(P => P.Brand)
                .WithMany()
                .HasForeignKey(P => P.BrandId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(P => P.Category)
                .WithMany()
                .HasForeignKey(P => P.CategoryId)
                .OnDelete(DeleteBehavior.SetNull);

            builder.HasMany(P => P.SubImages)
                .WithOne(I => I.Product)
                .HasForeignKey(I => I.ProductId)
                .OnDelete(DeleteBehavior.Cascade);
            builder.Ignore(P => P.Type);
        }
    }
}
