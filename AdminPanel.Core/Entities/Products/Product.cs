using AdminPanel.Core.Entities.Brands;
using AdminPanel.Core.Entities.Categories;

namespace AdminPanel.Core.Entities.Products
{
    public class Product : ModelBase
    {
        public string ProductName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string MainImage { get; set; } = string.Empty;
        public ICollection<ProductImages> SubImages { get; set; } = new HashSet<ProductImages>();
        public Brand? Brand { get; set; }
        public Category? Category { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        private int stock;

        public int Stock
        {
            get { return stock; }
            set { stock = Math.Max(0, value); }
        }
        public int MinimumStock { get; set; } = 10;

        public StockType Type
        {
            get {
                if (Stock <= 0) return StockType.OutOfStock;
                if (Stock <= MinimumStock) return StockType.LowStock;
                return StockType.InStock;
            }
        }

    }
}
