using AdminPanel.Core.Entities.Products;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Core.Specifications.ProductSpecifications
{
    public class ProductWithFilterSpec : BaseSpecifications<Product>
    {
        public ProductWithFilterSpec(ProductParams @params)
            :base(P => 
            (string.IsNullOrEmpty(@params.Search) || P.ProductName.Contains(@params.Search))&&
            (!@params.BrandId.HasValue || P.BrandId == @params.BrandId.Value)&&
            (!@params.CategoryId.HasValue || P.CategoryId == @params.CategoryId.Value)
            )
        {
            
        }
    }
}
