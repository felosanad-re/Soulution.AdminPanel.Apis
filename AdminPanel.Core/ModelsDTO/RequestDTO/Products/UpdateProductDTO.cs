using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Products
{
    public class UpdateProductDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        public IFormFile? MainImage { get; set; }
        public List<IFormFile>? SubImages { get; set; }
        public List<int>? DeletedImages { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
