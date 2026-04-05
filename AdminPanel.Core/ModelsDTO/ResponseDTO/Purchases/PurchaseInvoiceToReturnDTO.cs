namespace AdminPanel.Core.ModelsDto.ResponseDTO.Purchases
{
    public class PurchaseInvoiceToReturnDTO
    {
        public int Id { get; set; }
        public string AdminName { get; set; } // Account User
        public string CompanyName { get; set; }
        public List<PurchaseInvoiceItemsToReturnDTO> Items { get; set; } = new List<PurchaseInvoiceItemsToReturnDTO>();
        public decimal TotalPurchase { get; set; }
    }
}
