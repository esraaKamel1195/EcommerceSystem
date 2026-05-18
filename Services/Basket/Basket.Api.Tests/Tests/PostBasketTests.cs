using Basket.Application.Commands;
using Basket.Core.Entities;
using System.Net;
using System.Net.Http.Json;

namespace Basket.Api.Tests.Tests
{
    public class PostBasketTests: BasketApiTestBase
    {
        public PostBasketTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task PostBasket_WhenValid_Return200()
        {
            // Arrange
            var command = new CreateShoppingCartCommand("esraa", new List<ShoppingCartItem>()
                {
                    new ShoppingCartItem
                    {
                        productId = "1",
                        productName = "Test Product",
                        quantity = 2,
                        price = 10.0m,
                        ImageFile = "test.svg"
                    }
                }
            );

            // Act
            var response = await _httpClient.PostAsJsonAsync("/api/v1/BasketApi/CreateBasket", command);
            
           
            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }
}