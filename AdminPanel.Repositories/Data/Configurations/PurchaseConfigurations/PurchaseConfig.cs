using AdminPanel.Core.Entities.PurchaseInvoices;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPanel.Repositories.Data.Configurations.PurchaseConfigurations
{
    public class PurchaseConfig : IEntityTypeConfiguration<PurchaseInvoice>
    {
        public void Configure(EntityTypeBuilder<PurchaseInvoice> builder)
        {
            builder.Property(P => P.TotalPurchase).HasPrecision(18, 4);
        }
    }
}
