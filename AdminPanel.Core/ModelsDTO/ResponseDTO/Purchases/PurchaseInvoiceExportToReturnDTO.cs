using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Purchases
{
    public class PurchaseInvoiceExportToReturnDTO
    {
        public int Id { get; set; }
        public string UserName { get; set; } // Account User
        public string CompanyName { get; set; }
        public string Items { get; set; }
        public decimal TotalReportTransaction { get; set; }
        public DateTime CreatedAt { get; set; }
        public string CreatedBy { get; set; } = string.Empty;
    }
}
