using AdminPanel.Core.Entities.Products;

namespace AdminPanel.Core.Specifications.ProductSpecifications
{
    public class ProductSpec: BaseSpecifications<Product>
    {
        public ProductSpec(ProductParams @params)
            :base(P => 
                   (!@params.BrandId.HasValue || P.BrandId == @params.BrandId.Value)
                && (!@params.CategoryId.HasValue || P.CategoryId == @params.CategoryId.Value)
                && (string.IsNullOrEmpty(@params.Search) ||
                        P.ProductName.ToLower().Contains(@params.Search.ToLower()))
            )
        {
            AddIncluedes();

            AddPagination(@params.PageSize * (@params.PageIndex - 1), @params.PageSize);

            AddSorting(@params);
        }
        public ProductSpec(int id)
            : base(P => P.Id == id)
        {
            Includes.Add(P => P.Brand!);
            Includes.Add(P => P.Category!);
            Includes.Add(P => P.SubImages);
        }

        public ProductSpec(IEnumerable<int> productsId)
            : base(P => productsId.Contains(P.Id))
        {
            AddIncluedes();
        }

        public ProductSpec()
            : base()
        {
            AddIncluedes();
        }
        private void AddSorting(ProductParams @params)
        {
            if (!string.IsNullOrEmpty(@params.Sort))
            {
                switch (@params.Sort.ToLower())
                {
                    case "pricedesc":
                        AddOrderByDesc(P => P.Price);
                        break;
                    case "priceasc":
                        AddOrderBy(P => P.Price);
                        break;
                    case "stockasc":
                        AddOrderBy(P => P.Stock);
                        break;
                    case "stockdesc":
                        AddOrderByDesc(P => P.Stock);
                        break;
                    default:
                        AddOrderBy(P => P.ProductName);
                        break;
                }
            }
        }

        private void AddIncluedes()
        {
            Includes.Add(P => P.Brand!);
            Includes.Add(P => P.Category!);
            Includes.Add(P => P.SubImages);
        }

    }
}
