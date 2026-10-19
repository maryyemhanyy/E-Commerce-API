using E_Commerce.Application.Params.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Params
{
    public class ProductQueryParams
    {
        public int? BrandId { get; set; }
        public int? TypeId { get; set; }
        public string? SearchValue { get; set; }
        public ProductSortingOptions? SortingOption { get; set; }
        public int PageIndex { get; set; } = 1;

        private const int defaultPageSize = 5;
        private const int maxPageSize = 10;
        private int pageSize = defaultPageSize;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = value > maxPageSize ? maxPageSize : (value < 1  ? defaultPageSize : value);
        }

    }
}
