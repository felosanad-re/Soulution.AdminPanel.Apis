using AdminPanel.Core.Entities.PurchaseInvoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Repositories.Data.Configurations.PurchaseConfigurations
{
    public class PurchaseItemsConfig : IEntityTypeConfiguration<PurchaseInvoiceItems>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoiceItems> builder)
        {
            builder.Property(PI => PI.Price).HasPrecision(18, 4);
            builder.Property(PI => PI.TotalPrice).HasPrecision(18, 4);
            builder.HasOne(PI => PI.PurchaseInvoice)
                .WithMany(P => P.Items)
                .HasForeignKey(PI => PI.PurchaseInvoiceId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
