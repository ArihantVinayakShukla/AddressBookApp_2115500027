using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;

namespace RepositoryLayer.Helper
{
    public class RedisCacheHelper
    {
        private readonly IDistributedCache _cache;
        private readonly int _cacheExpirationMinutes;

        public RedisCacheHelper(IDistributedCache cache, IConfiguration configuration)
        {
            _cache = cache;
            _cacheExpirationMinutes = int.Parse(configuration["Redis:CacheExpirationInMinutes"]);
        }

        public async Task SetCacheAsync<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(_cacheExpirationMinutes)
            };

            var jsonData = JsonSerializer.Serialize(data);
            await _cache.SetStringAsync(key, jsonData, options);
        }

        public async Task<T> GetCacheAsync<T>(string key)
        {
            var jsonData = await _cache.GetStringAsync(key);
            return jsonData != null ? JsonSerializer.Deserialize<T>(jsonData) : default;
        }

        public async Task RemoveCacheAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
