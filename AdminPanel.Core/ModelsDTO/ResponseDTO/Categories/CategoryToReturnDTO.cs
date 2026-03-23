namespace AdminPanel.Core.ModelsDto.ResponseDTO.Categories
{
    public class CategoryToReturnDTO
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
