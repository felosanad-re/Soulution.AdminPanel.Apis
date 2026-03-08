namespace AdminPanel.Core.Entities.Brands
{
    public class Brand : ModelBase
    {
        public string BrandName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Logo { get; set; }
    }
}
