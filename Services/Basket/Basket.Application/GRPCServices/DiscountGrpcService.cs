using Discunt.Grpc.Protos;

namespace Basket.Application.GRPCServices
{
    public class DiscountGrpcService
    {
        private readonly DiscountProtoService.DiscountProtoServiceClient _discountProtoServiceClient;

        public DiscountGrpcService(DiscountProtoService.DiscountProtoServiceClient discountProtoServiceClient)
        {
            _discountProtoServiceClient = discountProtoServiceClient;
        }

        public async Task<CouponModel> GetDiscount(string productName)
        {
            GetDiscountRequest discountRequest = new GetDiscountRequest { ProductName = productName };
            return await _discountProtoServiceClient.GetDiscountAsync(discountRequest);
        }

        public async Task<CouponModel> CreateDiscount(CouponModel coupon)
        {
            CreateDiscountRequest createRequest = new CreateDiscountRequest { Coupon = coupon };
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
