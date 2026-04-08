namespace AdminPanel.Core.ModelsDto.ResponseDTO.Purchases
{
    public class PurchaseInvoiceToReturnDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } // Account User
        public string CompanyName { get; set; }
        public List<PurchaseInvoiceItemsToReturnDTO> Items { get; set; } = new List<PurchaseInvoiceItemsToReturnDTO>();
        public decimal TotalReportTransaction { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
