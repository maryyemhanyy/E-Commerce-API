using E_Commerce.Application.Common;
using E_Commerce.Application.DTOs.Baskets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface IBasketService
    {
        Task<Result<CustomerBasketDto>> GetBasketAsync(string basketId , CancellationToken ct = default);
        Task<Result<CustomerBasketDto>> CreateOrUpdateBasketAsync(CustomerBasketDto basket, CancellationToken ct = default);
        Task<Result<bool>> DeleteBasketAsync(string basketId, CancellationToken ct = default);
    }
}
