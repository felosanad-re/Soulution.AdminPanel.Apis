namespace AdminPanel.Core.Specifications
{
    public class ProductParams
    {
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public string? Sort { get; set; }
        public string? Search { get; set; }
        private int pageSize = 5;
        public int MaxPageSize { get; set; } = 10;
        public int PageSize
        {
            get { return pageSize; }
            set { pageSize = value > MaxPageSize? MaxPageSize : value; }
        }

        public int PageIndex { get; set; } = 1;
    }
}
