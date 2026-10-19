using E_Commerce.Application.Params;
using E_Commerce.Application.Params.Enum;
using E_Commerce.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class ProductSpecifications : BaseSpecifications<Product , int>
    {
        public ProductSpecifications(int id):base(p => p.Id == id)
        {
            AddInclude(p => p.Brand);
            AddInclude(p => p.Type);
        }
        public ProductSpecifications(ProductQueryParams queryParams)
            :base((p => (!queryParams.BrandId.HasValue || p.BrandId == queryParams.BrandId)
            && (!queryParams.TypeId.HasValue || p.TypeId == queryParams.TypeId)
            &&(string.IsNullOrEmpty(queryParams.SearchValue) ||( p.Name.ToLower().Contains(queryParams.SearchValue.ToLower())))))
        {
            AddInclude(p => p.Brand);
            AddInclude(p => p.Type);

            switch(queryParams.SortingOption)
            {
                case ProductSortingOptions.NameAsc: AddOrderByAsc(p => p.Name);break;
                case ProductSortingOptions.NameDesc: AddOrderByDesc(p => p.Name); break;
                case ProductSortingOptions.PriceAsc: AddOrderByAsc(p => p.Price); break;
                case ProductSortingOptions.PriceDesc: AddOrderByDesc(p => p.Price); break;
                _: AddOrderByAsc(p => p.Name); break;

            }
            ApplyPagination(queryParams.PageIndex, queryParams.PageSize);
        }

    }
}
