using AdminPanel.Core.ModelsDto.RequestDTO.Reports;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Reports
{
    public class SalesReportTransactionToReturnDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }

        public string CompanyName { get; set; }
        public List<SalesReportTransactionItemToReturnDTO> Items { get; set; } = new List<SalesReportTransactionItemToReturnDTO>();

        public decimal TotalReportTransaction { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
