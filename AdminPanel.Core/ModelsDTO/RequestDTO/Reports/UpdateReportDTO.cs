using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Reports
{
    public class UpdateReportDTO
    {
        [Required]
        public int Id { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "Items Can't Be Empty")]
        public IList<ReportTransactionItemDTO> Items { get; set; } = new List<ReportTransactionItemDTO>();
    }
}
