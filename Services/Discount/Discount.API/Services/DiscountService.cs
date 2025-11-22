using Discount.Application.Commands;
using Discunt.Grpc.Protos;
using MediatR;

namespace Discount.API.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IMediator _mediator;
        public DiscountService(IMediator mediator)
        {
            _mediator = mediator;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, Grpc.Core.ServerCallContext context)
        {
            var query = new Discount.Application.Queries.GetDiscountQuery(request.ProductName);
            return await _mediator.Send(query, context.CancellationToken);
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, Grpc.Core.ServerCallContext context)
        {
            var command = new CreateDiscountCouponCommand
            {
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount
            };

            var result = await _mediator.Send(command);
            return result;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, Grpc.Core.ServerCallContext context)
        {
            var command = new UpdateDiscountCouponCommand
            {
                Id = request.Coupon.Id,
                ProductName = request.Coupon.ProductName,
                Description = request.Coupon.Description,
                Amount = request.Coupon.Amount,
            };
            var result = await _mediator.Send(command, context.CancellationToken);
            return result;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, Grpc.Core.ServerCallContext context)
        {
            var command = new DeleteDiscountCouponCommand(request.ProductName);
            var result = await _mediator.Send(command, context.CancellationToken);
            return new DeleteDiscountResponse { Success = result };
        }
    }
}