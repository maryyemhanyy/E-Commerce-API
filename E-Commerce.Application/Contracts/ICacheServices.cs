using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Contracts
{
    public interface ICacheServices
    {
        Task<string?> GetAsync(string cacheKey, CancellationToken ct = default);
        Task SetAsync(string cacheKey, object value, TimeSpan ttl, CancellationToken ct = default);
    }
}
