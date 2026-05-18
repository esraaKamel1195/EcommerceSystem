using Discount.Grpc.Protos;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Ordering.API;
using Ordering.Application.Responses;
using System.Net.Http.Json;

namespace Ordering.Tests.ServiceToServiceTests
{
    public class OrderingToDiscountGrpcTests
    {
        [Fact]
        public async Task Ordering_Should_Apply_Discount_When_Discount_Service_Returns_Value()
        {
            // arrange
            var factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder => 
                {
                    builder.ConfigureServices((context, services) => {
                        services.AddGrpcClient<DiscountProtoService.DiscountProtoServiceClient>(cfg=> 
                        {
                            cfg.Address = new Uri("http://localhost:8002/");
                        });
                    });
                });

            var client = factory.CreateClient();

            var checkoutRequest = new
            {
                UserName = "swn",
                FirstName = "Sina",
                LastName = "Wang",
                Email = "test@email.com",
                AddressLine = "1st Street",
                Country = "China",
                TotalPrice = 1000,
                City = "trsst",
                ZibCode = "15263",
                CardName = "tes test",
                CardNumber = "12345678912345",
                Expiration = "20/30",
                CVV = "321",
                PaymentMethod = "Cash",
            };

            //act
            var checkoutResponse = await client.PostAsJsonAsync("/api/v1/orders", checkoutRequest);

            checkoutResponse.IsSuccessStatusCode.Should().BeTrue();

            //act - get orders
            var orderResponse = await client.GetAsync("/api/v1/Orders/testuser");
            orderResponse.IsSuccessStatusCode.Should().BeTrue();

            var orders = await orderResponse.Content.ReadFromJsonAsync<List<OrderResponse>>();

            orders.Should().NotBeNull();
            orders!.Single().TotalPrice.Should().BeLessThan(1000);
        }
    }
}
