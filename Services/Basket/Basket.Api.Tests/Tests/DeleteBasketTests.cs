using Basket.Application.Commands;
using Basket.Core.Entities;
using System.Net;
using System.Net.Http.Json;

namespace Basket.Api.Tests.Tests
{
    public class DeleteBasketTests: BasketApiTestBase
    {
        public DeleteBasketTests(CustomWebApplicationFactory factory) : base(factory)
        {
        }

        [Fact]
        public async Task DeleteBasket_RemoveBasket()
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

            var postResponse = await _httpClient.PostAsJsonAsync("/api/v1/BasketApi/CreateBasket", command);
            
            postResponse.EnsureSuccessStatusCode();

            // Act
            var deleteResponse = await _httpClient.DeleteAsync("/api/v1/BasketApi/DeleteBasket/esraa");
            
            deleteResponse.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);

            var getResponse = await _httpClient.GetAsync("/api/v1/BasketApi/GetBasket/esraa");

            getResponse.EnsureSuccessStatusCode();

            Assert.True(getResponse.IsSuccessStatusCode);

            Assert.Equal(HttpStatusCode.NoContent, getResponse.StatusCode);

            var body = await getResponse.Content.ReadAsStringAsync();

            Assert.True(string.IsNullOrEmpty(body));
        }

        [Fact]
        public async Task DeleteBasket_NonExisting_ReturnOk()
        {
            //Act
            var deleteResponse = await _httpClient.DeleteAsync("/api/v1/BasketApi/DeleteBasket/test");

            deleteResponse.EnsureSuccessStatusCode();

            // Assert
            Assert.Equal(HttpStatusCode.OK, deleteResponse.StatusCode);
        }
    }
}