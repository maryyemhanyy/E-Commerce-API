using E_Commerce.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Domain.Entities.Order
{
    public class Order : BaseEntity<Guid>
    {
        public string BuyerEmail { get; set; } = default!;
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
        public OrderAddress ShipToAddress { get; set; } = default!;
        public DeliveryMethod DeliveryMethod { get; set; } = default!;

        [ForeignKey("DeliveryMethod")]
        public int DeliveryMethodId { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public decimal Subtotal { get; set; }
        public decimal GetTotal() => Subtotal + (DeliveryMethod?.Price ?? 0);
        public string? PaymentIntentId { get; set; }

    }
}
