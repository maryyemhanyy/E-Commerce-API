using AutoMapper;
using E_Commerce.Application.Common;
using E_Commerce.Application.Contracts;
using E_Commerce.Application.DTOs.Baskets;
using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository basketRepository;
        private readonly IMapper mapper;

        public BasketService(IBasketRepository basketRepository , IMapper mapper)
        {
            this.basketRepository = basketRepository;
            this.mapper = mapper;
        }
        public async Task<Result<CustomerBasketDto>> CreateOrUpdateBasketAsync(CustomerBasketDto basket, CancellationToken ct = default)
        {
            var customerBasket = mapper.Map<CustomerBasket>(basket);
            var result =await basketRepository.CreateOrUpdateBasketAsync(customerBasket , TimeSpan.FromDays(1) , ct);
            
            return result is not null ? Result<CustomerBasketDto>.Ok(mapper.Map<CustomerBasketDto>(result)) : 
                Result<CustomerBasketDto>.Fail(Error.Failure("Basket.CreationOrUpdateFailed", "Failed to create or update the basket."));
        }

        public async Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default)
        {
            var result =await basketRepository.DeleteBasketAsync(basketId, ct);
            return result ? Result<bool>.Ok(true) : Result<bool>.Fail(Error.Failure("Basket.DeletionFailed", "Failed to delete the basket."));
        }

        public async Task<Result<CustomerBasketDto>> GetBasketAsync(string basketId, CancellationToken ct = default)
        {
            var result =await basketRepository.GetBasketAsync(basketId, ct);

            return result is not null ? Result<CustomerBasketDto>.Ok(mapper.Map<CustomerBasketDto>(result)) : 
                Result<CustomerBasketDto>.Fail(Error.NotFound("Basket.NotFound", "Basket not found."));
        }
    }
}
