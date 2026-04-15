using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Reports
{
    public class CreateSalesReportDTO
    {
        [Required]
        public string CompanyName { get; set; }
        [Required]
        [MinLength(1, ErrorMessage ="Items Can't Be Empty")]
        public IList<SalesReportTransactionItemDTO> Items { get; set; } = new List<SalesReportTransactionItemDTO>();
    }
}
