using E_Commerce.Application.Params;
using E_Commerce.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class ProductCountSpecifications : BaseSpecifications<Product , int>
    {
        public ProductCountSpecifications(ProductQueryParams queryParams)
            : base((p => (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId)
            && (!queryParams.TypeId.HasValue || p.TypeId == queryParams.TypeId)
            && (string.IsNullOrEmpty(queryParams.SearchValue) || (p.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))))
        {
            
        }
    }
}
