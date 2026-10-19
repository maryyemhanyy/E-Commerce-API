using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Product
{
    public class Product : BaseEntity<int>
    {
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string PictureUrl { get; set; } = null!;
        public decimal Price { get; set; }
        public ProductBrand Brand { get; set; } = default!;
        public ProductType Type { get; set; } = default!;
        [ForeignKey("Brand")]
        public int BrandId { get; set; }
        [ForeignKey("Type")]
        public int TypeId { get; set; }


    }
}
