namespace AdminPanel.Core.ModelsDto.ResponseDTO.Purchases
{
    public class PurchaseInvoiceItemExportToReturnDTO
    {
        public int PurchaseInvoiceId { get; set; }
        public int ItemId { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
