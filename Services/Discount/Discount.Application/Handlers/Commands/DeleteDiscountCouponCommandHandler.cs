using Discount.Application.Commands;
using Discount.Application.Handlers.Queries;
using Discount.Core.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Discount.Application.Handlers.Commands
{
    public class DeleteDiscountCouponCommandHandler: IRequestHandler<DeleteDiscountCouponCommand, bool>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly ILogger<GetDiscountQueryHandler> _logger;

        public DeleteDiscountCouponCommandHandler
        (
            IDiscountRepository discountRepository,
            ILogger<GetDiscountQueryHandler> logger
        )
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteDiscountCouponCommand request, CancellationToken cancellationToken)
        {
            var deleted = await _discountRepository.DeleteDiscount(request.ProductName);
            _logger.LogInformation($"Coupon for the {request.ProductName} is successfully deleted");
            return deleted;
        }
    }
}
