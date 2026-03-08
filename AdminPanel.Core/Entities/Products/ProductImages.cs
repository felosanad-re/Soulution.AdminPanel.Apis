namespace AdminPanel.Core.Entities.Products
{
    public class ProductImages : ModelBase
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string ImagesUrl { get; set; } = string.Empty;
    }
}
