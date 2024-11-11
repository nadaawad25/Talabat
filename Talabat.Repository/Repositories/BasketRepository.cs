using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Talabat.Core.Entities;
using Talabat.Core.Repositories.Contract;

namespace Talabat.Repository.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDatabase _database;
        public BasketRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _database = connectionMultiplexer.GetDatabase();
        }
        public async Task<bool> DeleteBasketAsync(string BasketId)
        {
           return await _database.KeyDeleteAsync(BasketId);
        }

        public async Task<CustomerBasket?> GetBasketAsync(string BasketId)
        {
            var basket = await _database.StringGetAsync(BasketId);

            return basket.IsNull ? null : JsonSerializer.Deserialize<CustomerBasket>(basket);    ;//json to object => Deserilize 
        }

        public async Task<CustomerBasket?> UpdateBasketAsync(CustomerBasket basket)
        {
            var JsonBasket = JsonSerializer.Serialize(basket);
            var CreatedOrUpdated  = await _database.StringSetAsync(basket.Id , JsonBasket , TimeSpan.FromDays(1)); //Key and Value and Expire date
            if (!CreatedOrUpdated)
                return null;
            return await GetBasketAsync(basket.Id);
        }

        
    }
}
