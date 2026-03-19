using Microsoft.AspNetCore.Http;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Brands
{
    public class UpdatedBrandDTO
    {
        public int Id { get; set; }
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Logo { get; set; }
    }
}
