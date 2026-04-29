using Discount.Grpc.Protos;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.GRPCServices
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
            //catch (RpcException ex)
            //{
            //    // Log the exception or handle it as needed
            //    Console.WriteLine($"Discount for product '{productName}' not found: {ex.Message}");
            //    _logger.LogInformation($"Discount get discount exception '{ex}' ");
            //    return null; // Return null or a default value if the discount is not found
            //}
            catch (RpcException ex) when (ex.StatusCode == StatusCode.NotFound)
            {
                // No discount exists — return null, no exception, no fake coupon
                _logger.LogInformation("No discount found for product '{ProductName}'.", productName);
                return null;
            }
            catch (RpcException ex) when (ex.StatusCode == StatusCode.Unavailable)
            {
                // Service is down — log warning, return null to not block the flow
                _logger.LogWarning("Discount service unavailable for product '{ProductName}'.", productName);
                return null;
            }
            catch (RpcException ex)
            {
                // Unexpected gRPC error — log it properly as an error
                _logger.LogError(ex, "Unexpected gRPC error fetching discount for '{ProductName}'. StatusCode: {StatusCode}",
                    productName, ex.StatusCode);
                return null;
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
