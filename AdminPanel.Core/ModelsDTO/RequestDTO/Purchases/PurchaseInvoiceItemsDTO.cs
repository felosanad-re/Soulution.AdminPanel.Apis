using AdminPanel.Core.Entities.PurchaseInvoices;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Purchases
{
    public class PurchaseInvoiceItemsDTO
    {
        [Required]
        public int ProductId { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public decimal Price { get; set; } // Buyer Price
        [Required]
        public int Quantity { get; set; }
    }
}
