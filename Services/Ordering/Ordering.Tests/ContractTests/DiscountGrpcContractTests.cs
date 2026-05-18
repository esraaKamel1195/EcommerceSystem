using Discount.Grpc.Protos;
using FluentAssertions;
using Grpc.Net.Client;

namespace Ordering.Tests.ContractTests
{
    public class DiscountGrpcContractTests
    {
        [Fact]
        public async Task GetDiscount_Should_Return_Valid_Response()
        {
            //Arrange
            using var channel = GrpcChannel.ForAddress("http://localhost:8002/");

            var client = new DiscountProtoService.DiscountProtoServiceClient(channel);

            //Act
            var response = await client.GetDiscountAsync(new GetDiscountRequest()
            {
                ProductName = "IPhone"
            });

            //Assert
            response.ProductName.Should().Be("No Discount");
            response.Amount.Should().BeGreaterThanOrEqualTo(0);
        }
    }
}
