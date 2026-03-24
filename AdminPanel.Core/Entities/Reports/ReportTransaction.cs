using AdminPanel.Core.Entities.Identity;

namespace AdminPanel.Core.Entities.Reports
{
    public class ReportTransaction : ModelBase
    {
        public ApplicationUser? ApplicationUser { get; set; } // NFP [One]
        public string? UserId { get; set; } = string.Empty;

        public ICollection<ReportTransactionItem> Items { get; set; } = new HashSet<ReportTransactionItem>(); // NFP [Many]
    }
}
