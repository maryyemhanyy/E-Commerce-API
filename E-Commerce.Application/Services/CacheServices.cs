using E_Commerce.Application.Contracts;
using E_Commerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace E_Commerce.Application.Services
{
    public class CacheServices : ICacheServices
    {
        private readonly ICacheRepository cacheRepository;

        public CacheServices(ICacheRepository cacheRepository)
        {
            this.cacheRepository = cacheRepository;
        }
        public async Task<string?> GetAsync(string cacheKey, CancellationToken ct = default)
        => await cacheRepository.GetAsync(cacheKey, ct);

        public Task SetAsync(string cacheKey, object value, TimeSpan ttl, CancellationToken ct = default)
        {
            var json = JsonSerializer.Serialize(value , new JsonSerializerOptions {PropertyNamingPolicy = JsonNamingPolicy.CamelCase } );
            return cacheRepository.SetAsync(cacheKey, json, ttl, ct);
        }
    }
}
