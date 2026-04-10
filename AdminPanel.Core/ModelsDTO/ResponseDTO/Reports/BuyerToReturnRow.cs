using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Reports
{
    public class BuyerToReturnRow
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string UserId { get; set; }

        public string CompanyName { get; set; }
        public string? Items { get; set; }

        public decimal TotalReportTransactionPrice { get; set; }

        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastModifiedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
        public string ModifiedBy { get; set; } = string.Empty;
    }
}
