using Basket.Application.Commands;
using Basket.Core.Entities;
using System.Net;
using System.Net.Http.Json;

namespace Basket.Api.Tests.Tests
{
    public class GetBasketTests : BasketApiTestBase
    {
        public GetBasketTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task GetBasket_WhenExist_Return200()
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
            });

            var postResponse = await _httpClient.PostAsJsonAsync("/api/v1/BasketApi/CreateBasket", command);

            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await _httpClient.GetAsync("/api/v1/BasketApi/GetBasket/esraa");

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task GetBasket_WhenExist_ReturnBasketJson()
        {
            // Arrange
            var command = new CreateShoppingCartCommand("esraa", new List<ShoppingCartItem>()
            //{
            //    new ShoppingCartItem
            //    {
            //        productId = "1",
            //        productName = "Test Product",
            //        quantity = 2,
            //        price = 10.0m
            //    }
            //}
            );
            var postResponse = await _httpClient.PostAsJsonAsync("/api/v1/BasketApi/CreateBasket", command);

            postResponse.EnsureSuccessStatusCode();
            // Act
            var response = await _httpClient.GetAsync("/api/v1/BasketApi/GetBasket/esraa");
            // Assert
            var basket = await response.Content.ReadFromJsonAsync<ShoppingCart>();

            Assert.NotNull(basket);

            Assert.Equal("esraa", basket.UserName);

            Assert.Empty(basket.Items);
        }
    }
}