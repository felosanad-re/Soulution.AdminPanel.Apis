using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Products
{
    public class CreateProductDTO
    {
        [Required]
        public string ProductName { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        [Required]
        public decimal Price { get; set; }
        [Required]
        public IFormFile MainImage { get; set; }
        [Required]
        public List<IFormFile> SubImages { get; set; }
        [Required]
        public int BrandId { get; set; }
        [Required]
        public int CategoryId { get; set; }
        [Required]
        public int Stock { get; set; }
    }
}
