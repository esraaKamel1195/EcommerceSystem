namespace Basket.Api.Tests
{
    public class BasketApiTestBase: IClassFixture<CustomWebApplicationFactory>
    {
        protected readonly HttpClient _httpClient;

        public BasketApiTestBase(CustomWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
        }
    }
}
