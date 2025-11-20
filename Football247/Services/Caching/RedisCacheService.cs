using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace Football247.Services.Caching
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDistributedCache _cache;

        public RedisCacheService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<T?> GetDataAsync<T>(string key)
        {
            var data = await _cache.GetStringAsync(key);
            if (data == null)
            {
                return default(T);
            }
            // Deserialize: Lấy chuỗi JSON từ Redis --> Dịch ngược lại thành Object C#
            return JsonSerializer.Deserialize<T>(data);
        }

        public async Task SetDataAsync<T>(string key, T data)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) 
            };

            // Serialize: Convert Object C# --> Chuỗi JSON (String)
            await _cache.SetStringAsync(key, JsonSerializer.Serialize(data), options);
        }

        public async Task RemoveDataAsync(string key)
        {
            await _cache.RemoveAsync(key);
        }
    }
}
