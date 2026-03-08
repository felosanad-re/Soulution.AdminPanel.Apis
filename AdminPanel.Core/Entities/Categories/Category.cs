namespace AdminPanel.Core.Entities.Categories
{
    public class Category:ModelBase
    {
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Image { get; set; }
    }
}
