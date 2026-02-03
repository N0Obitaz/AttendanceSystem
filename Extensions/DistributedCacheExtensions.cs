using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;

namespace AttendanceSystem.Extensions
{
    public static class DistributedCacheExtensions
    {
        public static async Task SetRecordAsync<T>(this IDistributedCache cache, 
            string recordId, 
            T data, 
            TimeSpan? absoluteExpiretime = null, 
            TimeSpan? unusedExpireTime = null)
        {
            var options = new DistributedCacheEntryOptions();

            options.AbsoluteExpirationRelativeToNow = absoluteExpiretime ?? TimeSpan.FromSeconds(60);
            options.SlidingExpiration = unusedExpireTime;

            var jsonData = JsonSerializer.Serialize(data);
            await cache.SetStringAsync(recordId, jsonData, options);


        }

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if(jsonData is null)
            {
                return default;
            }

            return JsonSerializer.Deserialize<T>(jsonData);
        }
    }
}
