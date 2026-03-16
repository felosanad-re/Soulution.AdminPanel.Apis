using AdminPanel.Core.Entities.Products;

namespace AdminPanel.Core.Specifications.ProductSpecifications
{
    public class ProductSpec: BaseSpecifications<Product>
    {
        public ProductSpec()
            :base()
        {
            Includes.Add(P => P.Brand!);
            Includes.Add(P => P.Category!);
            Includes.Add(P => P.SubImages);
        }

        public ProductSpec(int id)
            : base(P => P.Id == id)
        {
            Includes.Add(P => P.Brand!);
            Includes.Add(P => P.Category!);
            Includes.Add(P => P.SubImages);
        }

        public ProductSpec(IEnumerable<int> productsId)
            :base(P => productsId.Contains(P.Id))
        {
            
        }
    }
}
