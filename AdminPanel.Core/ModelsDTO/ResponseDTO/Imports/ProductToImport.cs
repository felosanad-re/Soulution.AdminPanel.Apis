using System.ComponentModel.DataAnnotations.Schema;

namespace AdminPanel.Core.ModelsDto.ResponseDTO.Imports
{
    public class ProductToImport
    {
        public int Id { get; set; }
        [Column("Product Name")]
        public string ProductName { get; set; } = string.Empty;
        [Column("Description")]
        public string Description { get; set; } = string.Empty;
        [Column("Price")]
        public decimal Price { get; set; }
        [Column("Main Image")]
        public string MainImage { get; set; } = string.Empty;
        // This must match the Excel header exactly when no explicit ColumnMapping is passed.
        [Column("Sub Images")]
        public string SubImages { get; set; } = string.Empty;
        [Column("Brand Id")]
        public int BrandId { get; set; }
        [Column("Category Id")]
        public int CategoryId { get; set; }
        [Column("Brand Name")]
        public string BrandName { get; set; } = string.Empty;
        [Column("Category Name")]
        public string CategoryName { get; set; } = string.Empty;
        [Column("Stock")]
        public int Stock { get; set; }
        [Column("Type")]
        public string Type { get; set; } = string.Empty;
    }
}
