using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Imports
{
    public class PurchaseInvoiceToImport
    {
        // Header names here should match the exported Excel columns so generic import can bind them automatically.
        [Column("Id")]
        public int Id { get; set; }

        [Column("UserName")]
        public string UserName { get; set; } = string.Empty;

        [Column("CompanyName")]
        public string CompanyName { get; set; } = string.Empty;

        [Column("Items")]
        public string Items { get; set; } = string.Empty;

        [Column("TotalReportTransaction")]
        public decimal TotalReportTransaction { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;
    }
}
