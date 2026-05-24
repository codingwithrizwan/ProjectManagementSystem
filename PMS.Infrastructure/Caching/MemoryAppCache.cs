using Microsoft.Extensions.Caching.Memory;
using PMS.Application.Interfaces;

namespace PMS.Infrastructure.Caching
{
    public class MemoryAppCache : IAppCache
    {
        private readonly IMemoryCache _cache;

        public MemoryAppCache(IMemoryCache cache)
        {
            _cache = cache;
        }

        public async Task<T> GetOrCreateAsync<T>(string key, TimeSpan ttl, Func<Task<T>> factory)
        {
            if (_cache.TryGetValue(key, out T? value) && value is not null)
            {
                return value;
            }

            var result = await factory();
            _cache.Set(key, result, ttl);
            return result;
        }

        public void Remove(string key)
        {
            _cache.Remove(key);
        }
    }
}