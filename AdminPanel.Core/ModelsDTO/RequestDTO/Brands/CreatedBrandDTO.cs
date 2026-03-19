using Microsoft.AspNetCore.Http;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Brands
{
    public class CreatedBrandDTO
    {
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
