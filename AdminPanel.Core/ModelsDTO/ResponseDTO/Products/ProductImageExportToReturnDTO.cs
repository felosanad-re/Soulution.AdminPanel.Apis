namespace AdminPanel.Core.ModelsDto.ResponseDTO.Products
{
    public class ProductImageExportToReturnDTO
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string ImageType { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}
