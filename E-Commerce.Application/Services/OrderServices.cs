using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Orders;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Order;
using E_Commerce.Domain.Entities.Product;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class OrderServices : IOrderServices
    {
        private readonly IMapper mapper;
        private readonly IUnitOfWork unitOfWork;
        private readonly IBasketRepository basketRepository;

        public OrderServices(IMapper mapper , IUnitOfWork unitOfWork , IBasketRepository basketRepository )
        {
            this.mapper = mapper;
            this.unitOfWork = unitOfWork;
            this.basketRepository = basketRepository;
        }
        public async Task<Result<OrderToReturnDto>> CreateOrderAsync(OrderDto orderDto, string email, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(orderDto.BasketId, ct);
            if (basket == null) return Result<OrderToReturnDto>.Fail(Error.NotFound("Basket.NotFound", "Basket not found"));
            if(basket.BasketItems.Count == 0) return Result<OrderToReturnDto>.Fail(Error.Validation("Basket.Empty", "Basket is empty"));


            var productRepo = unitOfWork.GetRepositoryAsync<Product,int>();
            var productIds = basket.BasketItems.Select(i => i.Id).ToHashSet();
            var products = await productRepo.GetAllWithSpecificationsAsync(new ProductWithIdsSpecifications(productIds), ct);


            var orderItems = new List<OrderItem>(basket.BasketItems.Count);
            foreach(var item in basket.BasketItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.Id);
                if(product == null) return Result<OrderToReturnDto>.Fail(Error.NotFound("Product.NotFound", "Product not found"));

                orderItems.Add(new OrderItem
                {
                    Price = product.Price,
                    Quantity = item.Quantity,
                    ItemOrdered = new ProductItemOrdered()
                    {
                        ProductId = product.Id,
                        PictureUrl = product.PictureUrl,
                        ProductName = product.Name,
                    }
                });
            }


            var orderAddress = mapper.Map<OrderAddress>(orderDto.ShippingAddress);

            var deliveryMethod = await unitOfWork.GetRepositoryAsync<DeliveryMethod,int>().GetByIdAsync(orderDto.DeliveryMethodId , ct);
            if(deliveryMethod == null) return Result<OrderToReturnDto>.Fail(Error.NotFound("DeliveryMethod.NotFound", "DeliveryMethod not found"));

            var subTotal = orderItems.Sum(i=>i.Price * i.Quantity);

            var order = new Order()
            {
                BuyerEmail = email,
                OrderItems = orderItems,
                DeliveryMethod = deliveryMethod,
                ShipToAddress = orderAddress,
                DeliveryMethodId = deliveryMethod.Id,
                Subtotal = subTotal,
                PaymentIntentId = basket.PaymentIntentId
            };

            unitOfWork.GetRepositoryAsync<Order,Guid>().Add(order);
            var result = await unitOfWork.SaveChangesAsync();

            if (result <= 0) return Result<OrderToReturnDto>.Fail(Error.Failure("Order.Failure" , "Order Can't be created"));

            await basketRepository.DeleteBasketAsync(orderDto.BasketId, ct);

            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));
        }

        public async Task<Result<IReadOnlyList<DeliveryMethodDto>>> GetAllDeliveryMethodsAsync(CancellationToken ct = default)
        {
           var deliveryMethods =await unitOfWork.GetRepositoryAsync<DeliveryMethod,int>().GetAllAsync(ct);
           return Result<IReadOnlyList<DeliveryMethodDto>>.Ok(mapper.Map<IReadOnlyList<DeliveryMethodDto>>(deliveryMethods));
        }

        public async Task<Result<IReadOnlyList<OrderToReturnDto>>> GetAllOrdersAsync(string email, CancellationToken ct = default)
        {
            var orders =await unitOfWork.GetRepositoryAsync<Order,Guid>().GetAllWithSpecificationsAsync(new OrderSpecifications(email), ct);
            return Result<IReadOnlyList<OrderToReturnDto>>.Ok(mapper.Map<IReadOnlyList<OrderToReturnDto>>(orders));
        }

        public async Task<Result<OrderToReturnDto>> GetOrderByIdAsync(Guid orderId, string email, CancellationToken ct = default)
        {
            var order = await unitOfWork.GetRepositoryAsync<Order, Guid>().GetByIdWithSpecificationsAsync(new OrderSpecifications(orderId,email), ct);
            if(order == null) return Result<OrderToReturnDto>.Fail(Error.NotFound("Order.NotFound", "Order not found"));
            return Result<OrderToReturnDto>.Ok(mapper.Map<OrderToReturnDto>(order));
        }
    }
}
