using AutoMapper;
using Discount.Application.Commands;
using Discount.Application.Handlers.Queries;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Discount.Grpc.Protos;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Discount.Application.Handlers.Commands
{
    public class CreateDiscountCouponCommandHandler: IRequestHandler<CreateDiscountCouponCommand, CouponModel>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDiscountQueryHandler> _logger;

        public CreateDiscountCouponCommandHandler
        (
            IDiscountRepository discountRepository,
            IMapper mapper,
            ILogger<GetDiscountQueryHandler> logger
        )
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CouponModel> Handle(CreateDiscountCouponCommand request, CancellationToken cancellationToken)
        {
            var coupon = new Coupon
            { 
              ProductName = request.ProductName,
              Description = request.Description,
              Amount = request.Amount
            };
            var cretedCoupon = await _discountRepository.CreateDiscount(coupon);
            _logger.LogInformation($"Coupon for the {request.ProductName} is successfully created");
            var couponModel = _mapper.Map<CouponModel>(cretedCoupon);

            return couponModel;
        }
    }
}
