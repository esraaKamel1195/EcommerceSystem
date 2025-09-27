using Basket.Core.Entities;
using Basket.Core.Repositories;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;


namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository : IBasketRepository
    {
        private readonly IDistributedCache _Rediscache;

        public BasketRepository(IDistributedCache redisCah)
        {
            _Rediscache = redisCah;
        }
        public async Task<ShoppingCart> GetBasket(string username)
        {
            var basket = await _Rediscache.GetStringAsync(username);
            if (string.IsNullOrEmpty(basket))
            {
                return null;
            }
            return JsonConvert.DeserializeObject<ShoppingCart>(basket);
        }

        public async Task<ShoppingCart> UpdateBasket(ShoppingCart cart)
        {
            var basket = await _Rediscache.GetStringAsync(cart.UserName);
            if(basket != null)
            {
                return await GetBasket(cart.UserName);
            }
            else
            {
                await _Rediscache.SetStringAsync(cart.UserName, JsonConvert.SerializeObject(cart));
                return await GetBasket(cart.UserName);
            }
        }

        public async Task DeleteBasket(string username)
        {
            var basket = await _Rediscache.GetStringAsync(username);
            if(basket != null) 
            {
                await _Rediscache.RemoveAsync(basket);
            }
        }
    }
}
