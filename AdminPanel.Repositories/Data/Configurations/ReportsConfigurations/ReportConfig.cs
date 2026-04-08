using AdminPanel.Core.Entities.Reports;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AdminPanel.Repositories.Data.Configurations.ReportsConfigurations
{
    public class ReportConfig : IEntityTypeConfiguration<ReportTransaction>
    {
        public void Configure(EntityTypeBuilder<ReportTransaction> builder)
        {
            builder.Property(R => R.CreatedAt).HasColumnType("datetime2(0)");
            builder.Property(R => R.LastModifiedAt).HasColumnType("datetime2(0)");
            builder.Property(R => R.TotalReportTransaction).HasPrecision(18, 2);
            builder.HasOne(R => R.ApplicationUser)
                .WithMany()
                .HasForeignKey(R => R.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(R => R.Items)
                .WithOne(RI => RI.ReportTransaction)
                .HasForeignKey(RI => RI.ReportTransactionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
