using AdminPanel.Core.ModelsDto.RequestDTO.Reports;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Reports
{
    public class ReportTransactionToReturnDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }

        public List<ReportTransactionItemToReturnDTO> Items { get; set; } = new List<ReportTransactionItemToReturnDTO>();

        public decimal TotalReportTransactionPrice => Items.Sum(i => i.TotalPrice);

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
