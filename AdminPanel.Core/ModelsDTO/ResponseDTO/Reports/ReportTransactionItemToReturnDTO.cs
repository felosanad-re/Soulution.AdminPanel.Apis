using AdminPanel.Core.Entities.Products;
using AdminPanel.Core.Entities.Reports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Reports
{
    public class SalesReportTransactionItemToReturnDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Quantity { get; set; } // count of Sales
        public decimal TotalPrice { get; set; }
    }
}
