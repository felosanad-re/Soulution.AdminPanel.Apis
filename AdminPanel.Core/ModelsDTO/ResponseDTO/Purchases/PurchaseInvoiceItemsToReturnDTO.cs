namespace AdminPanel.Core.ModelsDto.ResponseDTO.Purchases
{
    public class PurchaseInvoiceItemsToReturnDTO
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Price { get; set; } // Buyer Price
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
