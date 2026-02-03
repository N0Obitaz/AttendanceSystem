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

            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            };
            var jsonData = JsonSerializer.Serialize(data, jsonOptions);
            await cache.SetStringAsync(recordId, jsonData, options);


        }

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string recordId)
        {
            var jsonData = await cache.GetStringAsync(recordId);

            if(jsonData is null)
            {
                return default;
            }
            var jsonOptions = new JsonSerializerOptions
            {
                ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles,
            };
            return JsonSerializer.Deserialize<T>(jsonData, jsonOptions);
        }
    }
}
