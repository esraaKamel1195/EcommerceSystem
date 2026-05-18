using Basket.Application.GRPCServices;
using Basket.Core.Repositories;
using Discount.Grpc.Protos;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Basket.Api.Tests
{
    public class CustomWebApplicationFactory: WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // remove real repositories and add in-memory repositories for testing
            builder.ConfigureServices(services => 
            {
                // --- Fix IDiscountGrpcService: remove ALL existing, add ONE singleton mock ---
                var grpcDescriptors = services
                    .Where(d => d.ServiceType == typeof(IDiscountGrpcService))
                    .ToList();
                foreach (var d in grpcDescriptors)
                    services.Remove(d);

                // Mock the gRPC service
                var mockDiscountGrpcService = new Mock<IDiscountGrpcService>();
                mockDiscountGrpcService
                    .Setup(x => x.GetDiscount(It.IsAny<string>()))
                    .ReturnsAsync(new CouponModel { Amount = 0 }); // or null

                services.AddSingleton(mockDiscountGrpcService.Object);

                var descriptor2 = services.SingleOrDefault(
                    d => d.ServiceType == typeof(IBasketRepository)
                );

                services.Remove(descriptor2);

                services.AddSingleton<IBasketRepository, FakeBasketRepository>();
            });
        }
    }
}