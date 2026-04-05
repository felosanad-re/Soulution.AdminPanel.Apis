namespace AdminPanel.Core.ModelsDto.RequestDTO.Purchases
{
    public class CreatePurchaseDTO
    {
        public string CompanyName { get; set; }

        public List<PurchaseInvoiceItemsDTO> Items { get; set; } = new List<PurchaseInvoiceItemsDTO>();
    }
}
