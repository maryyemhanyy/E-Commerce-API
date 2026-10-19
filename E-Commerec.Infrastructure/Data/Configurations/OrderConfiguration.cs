using E_Commerce.Domain.Entities.Order;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.Data.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
            builder.Property(o => o.BuyerEmail).IsRequired().HasMaxLength(250);
            builder.Property(builder => builder.Status).HasConversion<string>().HasMaxLength(50);
            builder.OwnsOne(o => o.ShipToAddress);
        }
    }
}
