using E_Commerce.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    public class ProductWithIdsSpecifications : BaseSpecifications<Product,int>
    {
        public ProductWithIdsSpecifications(IEnumerable<int> Ids) : base(p => Ids.Contains(p.Id))
        {
            
        }
    }
}
