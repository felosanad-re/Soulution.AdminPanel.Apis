namespace AdminPanel.Core.Entities.PurchaseInvoices
{
    public class PurchaseInvoiceItems
    {
        public int Id { get; set; }
        public PurchaseInvoice PurchaseInvoice { get; set; } // NFP [ONE]
        public int PurchaseInvoiceId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal GetTotalPrice()
            => TotalPrice = Price * Quantity;
    }
}
