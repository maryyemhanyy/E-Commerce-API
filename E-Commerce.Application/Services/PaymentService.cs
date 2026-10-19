using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Application.Specifications;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Order;
using E_Commerce.Domain.Entities.Product;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly IPaymentGateway paymentGateway;
        private readonly PaymentGatewaySettings stripeSettings;

        public PaymentService(IBasketRepository basketRepository , IUnitOfWork unitOfWork , IMapper mapper ,
            IPaymentGateway paymentGateway , IOptions<PaymentGatewaySettings> stripeSettings )
        {
            this.basketRepository = basketRepository;
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.paymentGateway = paymentGateway;
            this.stripeSettings = stripeSettings.Value;
        }
        public async Task<Result<CustomerBasketDto>> CreateOrUpdatePaymentIntentAsync(string basketId, CancellationToken ct = default)
        {
            var basket = await basketRepository.GetBasketAsync(basketId , ct);
            if (basket == null) return Result<CustomerBasketDto>.Fail(Error.NotFound("basket not found", $"basket with id {basketId} not found"));
            if(basket.BasketItems.Count == 0) return Result<CustomerBasketDto>.Fail(Error.Validation("basket is empty", $"basket with id {basketId} is empty"));

            var productRepo = unitOfWork.GetRepositoryAsync<Product, int>();
            var productIds = basket.BasketItems.Select(i => i.Id).ToHashSet();
            var products = await productRepo.GetAllWithSpecificationsAsync(new ProductWithIdsSpecifications(productIds));

            foreach(var item in basket.BasketItems)
            {
                var product = products.FirstOrDefault(p => p.Id == item.Id);
                if (product == null) return Result<CustomerBasketDto>.Fail(Error.NotFound("product not found", $"product with id {item.Id} not found"));

                item.Price = product.Price;
            }

            var deliveryMethodRepo = unitOfWork.GetRepositoryAsync<DeliveryMethod, int>();
            var deliveryMethod = await deliveryMethodRepo.GetByIdAsync(basket.DeliveryMethodId.Value, ct);
            if(deliveryMethod == null) return Result<CustomerBasketDto>.
                    Fail(Error.NotFound("Delivery Method not found", $"Delivery Method with id {basket.DeliveryMethodId.Value} not found"));

            basket.ShippingPrice = deliveryMethod.Price;

            var subTotal = basket.BasketItems.Sum(i => i.Quantity * i.Price);

            var total = subTotal + deliveryMethod.Price;

            var amount = checked((long)(Math.Round(total, 2, MidpointRounding.AwayFromZero) * 100m));

            if (!string.IsNullOrEmpty(basket.PaymentIntentId))
            {
               var result =  await paymentGateway.UpdatePaymentIntentAsync(basket.PaymentIntentId, amount, ct);
               basket.PaymentIntentId = result.PaymentIntentId;
               basket.ClientSecret = result.ClientSecret;
            }
            else
            {
                var result = await paymentGateway.CreatePaymentIntentAsync(amount, stripeSettings.DefaultCurrency ,ct);
                basket.PaymentIntentId = result.PaymentIntentId;
                basket.ClientSecret = result.ClientSecret;
            }

            await basketRepository.CreateOrUpdateBasketAsync(basket,ct:ct);
            return Result<CustomerBasketDto>.Ok(mapper.Map<CustomerBasketDto>(basket));
        }

        public async Task PaymentFailed(string paymentIntentId)
        {
            var orderRepo = unitOfWork.GetRepositoryAsync<Order, Guid>();
            var order = await orderRepo.GetByIdWithSpecificationsAsync(new PaymentIntentSpecifications(paymentIntentId));
            if (order == null) return;

            order.Status = OrderStatus.PaymentFailed;
            await unitOfWork.SaveChangesAsync();
        }

        public async Task PaymentSucceeded(string paymentIntentId)
        {
            var orderRepo = unitOfWork.GetRepositoryAsync<Order, Guid>();
            var order = await orderRepo.GetByIdWithSpecificationsAsync(new PaymentIntentSpecifications(paymentIntentId));
            if (order == null) return;

            order.Status = OrderStatus.PaymentReceived;
            await unitOfWork.SaveChangesAsync();
        }
    }

    public class PaymentGatewaySettings
    {
        public string SecretKey { get; set; } = default!;
        public string DefaultCurrency { get; set; } = "USD";
        public string WebhookSecret { get; set; } = default!;

    }
}
