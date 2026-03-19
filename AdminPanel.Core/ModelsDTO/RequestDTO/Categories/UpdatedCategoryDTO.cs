using Microsoft.AspNetCore.Http;

namespace AdminPanel.Core.ModelsDto.RequestDTO.Categories
{
    public class UpdatedCategoryDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
