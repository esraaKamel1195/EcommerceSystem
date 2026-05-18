using Basket.Core.Entities;
using Basket.Core.Repositories;

namespace Basket.Api.Tests
{
    public class FakeBasketRepository : IBasketRepository
    {
        private readonly Dictionary<string, ShoppingCart> _baskets = new();

        public Task<ShoppingCart> GetBasket(string username)
        {
            if (_baskets.TryGetValue(username, out var cart))
            {
                return Task.FromResult(cart);
            } else
            {
                return Task.FromResult<ShoppingCart>(null);
            }
        }

        public Task<ShoppingCart> UpdateBasket(ShoppingCart cart)
        {
            _baskets[cart.UserName] = cart;
            return Task.FromResult(cart);
        }

        public Task DeleteBasket(string username)
        {
            _baskets.Remove(username);
            return Task.CompletedTask;
        }
    }
}
