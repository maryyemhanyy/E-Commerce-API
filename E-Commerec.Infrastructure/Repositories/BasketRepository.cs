using E_Commerce.Domain.Contracts;
using E_Commerce.Domain.Entities.Basket;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerec.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connection)
        {
            _database = connection.GetDatabase();
        }
        public async Task<CustomerBasket?> CreateOrUpdateBasketAsync(CustomerBasket basket, TimeSpan? ttl = null, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(basket);

            var success = await _database.StringSetAsync(basket.Id, json, ttl ?? TimeSpan.FromMinutes(30));

            return success ? basket : null;
        }

        public async Task<bool> DeleteBasketAsync(string basketId, CancellationToken ct = default)
         => await _database.KeyDeleteAsync(basketId);

        public async Task<CustomerBasket?> GetBasketAsync(string basketId, CancellationToken ct = default)
        {
            var basket =await _database.StringGetAsync(basketId);

            if(basket.IsNullOrEmpty) return null;

            return JsonSerializer.Deserialize<CustomerBasket>(basket.ToString());
        }
    }
}
