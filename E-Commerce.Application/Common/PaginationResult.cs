using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Common
{
    public class PaginationResult<TEntity>
    {
        public PaginationResult(IReadOnlyList<TEntity> data, int pageIndex , int pageSize , int totalCount) { 
            Data = data;
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }
        public IReadOnlyList<TEntity> Data { get; set; } = [];
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }

    }
}
