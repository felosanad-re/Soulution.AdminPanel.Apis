namespace AdminPanel.Core.Entities.PurchaseInvoices
{
    public class PurchaseInvoice : ModelBase
    {
        public string UserName { get; set; } // Admin
        public string CompanyName { get; set; }

        // NFP[Many]
        public ICollection<PurchaseInvoiceItems> Items { get; set; } = new HashSet<PurchaseInvoiceItems>();
        public decimal TotalReportTransaction { get; set; }
        public decimal GetTotalPurchase() => TotalReportTransaction = Items.Sum(i => i.TotalPrice);
    }
}
