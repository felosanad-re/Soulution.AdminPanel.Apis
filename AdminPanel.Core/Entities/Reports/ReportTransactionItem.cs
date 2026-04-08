using AdminPanel.Core.Entities.Products;

namespace AdminPanel.Core.Entities.Reports
{
    public class ReportTransactionItem
    {
        public int Id { get; set; }
        public ReportTransaction? ReportTransaction { get; set; }
        public int? ReportTransactionId { get; set; }
        public Product? Product { get; set; } // NFP [One]
        public int? ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } // count of Sales
        public decimal TotalPrice { get; set; } = 0;
        public void GetTotalPrice()
        {
            TotalPrice = Price * Quantity;
        }
    }
}
