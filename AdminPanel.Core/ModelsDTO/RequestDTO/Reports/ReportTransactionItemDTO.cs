using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Reports
{
    public class ReportTransactionItemDTO
    {
        [Required]
        public int ProductId { get; set; }

        [Required]
        public int Quantity { get; set; } // count of Sales
    }
}
