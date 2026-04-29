using Discount.Application.Commands;
using Discount.Application.Queries;
using Discount.Grpc.Protos;
using Grpc.Core;
using MediatR;

namespace Discount.API.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<DiscountService> _logger;
        public DiscountService(IMediator mediator, ILogger<DiscountService> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var query = new GetDiscountQuery(request.ProductName);
            var result = await _mediator.Send(query);
            _logger.LogInformation($"trying to query to get discount {result} from discount api service");
            return result;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
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

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
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

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var command = new DeleteDiscountCouponCommand(request.ProductName);
            var result = await _mediator.Send(command, context.CancellationToken);
            return new DeleteDiscountResponse { Success = result };
        }
    }
}