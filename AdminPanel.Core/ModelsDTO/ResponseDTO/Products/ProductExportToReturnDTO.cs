using AdminPanel.Core.ModelsDto.RequestDTO.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Products
{
    public class ProductExportToReturnDTO
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string MainImage { get; set; } = string.Empty;
        public string SubImages { get; set; }
        public int BrandId { get; set; }
        public int CategoryId { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string CategoryName { get; set; } = string.Empty;
        public int Stock { get; set; }
        public string Type { get; set; } = string.Empty;
    }
}
