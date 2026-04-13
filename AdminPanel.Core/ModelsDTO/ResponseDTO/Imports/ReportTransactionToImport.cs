using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Imports
{
    public class ReportTransactionToImport
    {
        // Header names here should match the exported Excel columns so generic import can bind them automatically.
        [Column("Id")]
        public int Id { get; set; }

        [Column("UserName")]
        public string UserName { get; set; } = string.Empty;

        [Column("UserId")]
        public string UserId { get; set; } = string.Empty;

        [Column("CompanyName")]
        public string CompanyName { get; set; } = string.Empty;

        [Column("Items")]
        public string Items { get; set; } = string.Empty;

        [Column("TotalReportTransactionPrice")]
        public decimal TotalReportTransactionPrice { get; set; }

        [Column("IsDeleted")]
        public bool IsDeleted { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("LastModifiedAt")]
        public DateTime LastModifiedAt { get; set; }

        [Column("CreatedBy")]
        public string CreatedBy { get; set; } = string.Empty;

        [Column("ModifiedBy")]
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
