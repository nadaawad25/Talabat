using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Services.Contract;

namespace Talabat.Service.Services
{
    public class ResponseCacheService : IResponseCacheService
    {
        private readonly IDatabase _database;

        public ResponseCacheService(IConnectionMultiplexer Redis)
        {
            _database = Redis.GetDatabase();
        }
        public async Task  CacheResponseAsync(string CacheKey, object Response, TimeSpan ExpireTime)
        {
            if (Response is null)
                return;

            var options = new JsonSerializerOptions()
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var JsonResponse = JsonSerializer.Serialize(Response , options);
            await _database.StringSetAsync(CacheKey , JsonResponse ,ExpireTime);   //Pascal and i wnat Camel 

        }

        public async Task<string?> GetCachedResponse(string CacheKey)
        {
            var CachedResponse = await _database.StringGetAsync(CacheKey);
            if (CachedResponse.IsNullOrEmpty)
                return null;  

            return CachedResponse;
        }

    }
}
