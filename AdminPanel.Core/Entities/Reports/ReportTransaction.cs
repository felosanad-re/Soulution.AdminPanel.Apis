using AdminPanel.Core.Entities.Identity;

namespace AdminPanel.Core.Entities.Reports
{
    public class ReportTransaction : ModelBase
    {
        public ApplicationUser? ApplicationUser { get; set; } // NFP [One]
        public string? UserId { get; set; } = string.Empty;
        public string? CompanyName { get; set; }

        public ICollection<ReportTransactionItem> Items { get; set; } = new HashSet<ReportTransactionItem>(); // NFP [Many]

        public decimal TotalReportTransaction { get; set; } = 0;

        public void GetTotalReportTransactionPrice()
        {
            TotalReportTransaction = Items.Sum(i => i.TotalPrice);
        }
    }
}
