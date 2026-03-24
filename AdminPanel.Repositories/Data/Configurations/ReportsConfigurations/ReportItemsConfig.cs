using AdminPanel.Core.Entities.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPanel.Repositories.Data.Configurations.ReportsConfigurations
{
    public class ReportItemsConfig : IEntityTypeConfiguration<ReportTransactionItem>
    {
        public void Configure(EntityTypeBuilder<ReportTransactionItem> builder)
        {
            builder.Property(RI => RI.Price).HasColumnType("decimal(18, 2)");
        }
    }
}
