using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Talabat.Core.Entities;

namespace Talabat.Core.Specifications
{
    public class ProductWithBrandAndTypeSpecification :BaseSpecification<Product>
    {
        public ProductWithBrandAndTypeSpecification(ProductSpecParams specParams)
            : base( p => 
                    (string.IsNullOrEmpty(specParams.Search)||p.Name.ToLower().Contains(specParams.Search))&&
                    (!specParams.BrandId.HasValue || p.ProductBrandId == specParams.BrandId) && 
                    (!specParams.TypeId.HasValue || p.ProductTypeId == specParams.TypeId))
        {
            Includes.Add(p => p.ProductBrand); // Include the Brand
            Includes.Add(p => p.ProductType);   // Include the Type
            if (!string.IsNullOrEmpty(specParams.Sort))
            {
                switch (specParams.Sort)
                {
                    case "PriceAsyc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "PriceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;
                }
            }

            ApplyPagination(specParams.PageSize * (specParams.PageIndex - 1), specParams.PageSize);

        }
        public ProductWithBrandAndTypeSpecification(int id) : base( product => product.Id == id)
        {
            Includes.Add(product => product.ProductBrand);
            Includes.Add(product => product.ProductType);
        }
    }
}
