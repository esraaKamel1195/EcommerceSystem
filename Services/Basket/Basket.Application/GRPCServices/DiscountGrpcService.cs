using Discount.Grpc.Protos;
using Microsoft.Extensions.Logging;

namespace Basket.Application.GRPCServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoServiceClient;
        private readonly ILogger<DiscountGrpcService> _logger;
        public DiscountGrpcService(
            DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient,
            ILogger<DiscountGrpcService> logger
        )
        {
            _discountProtoServiceClient = discountProtoServiceClient;
            _logger = logger;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            try
            {
                GetDiscountRequest discountRequest = new GetDiscountRequest { ProductName = productName };
                return await _discountProtoServiceClient.GetDiscountAsync(discountRequest);
            }
            catch (Grpc.Core.RpcException ex)
            {
                // Log the exception or handle it as needed
                Console.WriteLine($"Discount for product '{productName}' not found: {ex.Message}");
                _logger.LogInformation($"Discount get discount exception '{ex}' ");
                return new CouponModel { ProductName = productName, Amount = 0 }; // Return null or a default value if the discount is not found
            }
        }

        public async Task<CouponModel> CreateDiscount(CouponModel coupon)
        {
            CreateDiscountRequest createRequest = new CreateDiscountRequest { Coupon = coupon };
            //_logger.LogInformation($"Discount get discount exception '{ex}' ");
            return await _discountProtoServiceClient.CreateDiscountAsync(createRequest);
        }

        public async Task<CouponModel> UpdateDiscount(CouponModel coupon)
        {
            var updateRequest = new UpdateDiscountRequest { Coupon = coupon };
            return await _discountProtoServiceClient.UpdateDiscountAsync(updateRequest);
        }

        public async Task<bool> DeleteDiscount(string productName)
        {
            DeleteDiscountRequest discountRequest = new DeleteDiscountRequest { ProductName = productName };
            var response = await _discountProtoServiceClient.DeleteDiscountAsync(discountRequest);
            return response.Success;
        }
    }
}
