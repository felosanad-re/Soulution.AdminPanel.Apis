using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Reports
{
    public class CreateReportDTO
    {
        [Required]
        [MinLength(1, ErrorMessage ="Items Can't Be Empty")]
        public IList<ReportTransactionItemDTO> Items { get; set; } = new List<ReportTransactionItemDTO>();
    }
}
