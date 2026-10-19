using E_Commerce.Domain.Entities.Product;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Data.Configurations
{ 
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
           builder.Property(p => p.Name).IsRequired().HasMaxLength(100);
           builder.Property(p => p.Description).IsRequired().HasMaxLength(300);
           builder.Property(p => p.PictureUrl).IsRequired().HasMaxLength(200);
           builder.Property(p => p.Price).IsRequired().HasColumnType("decimal(10,2)");


        }
    }
}
