using AutoMapper;
using Discount.Application.Handlers.Queries;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Discunt.Grpc.Protos;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Application.Commands
{
   // Handler should implement IRequestHandler<UpdateCouponCommand, CouponModel>
    public class UpdateDiscountCouponCommandHandler : IRequestHandler<UpdateDiscountCouponCommand, CouponModel>
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetDiscountQueryHandler> _logger;

        public UpdateDiscountCouponCommandHandler
        (
            IDiscountRepository discountRepository,
            ILogger<GetDiscountQueryHandler> logger
        )
        {
            _discountRepository = discountRepository;
            _logger = logger;
        }
        public async Task<CouponModel> Handle(UpdateDiscountCouponCommand request, CancellationToken cancellationToken)
        {
            // Example implementation, replace with actual update logic
            var updatedCoupon = new Coupon
            {
                Id = request.Id,
                ProductName = request.ProductName,
                Description = request.Description,
                Amount = request.Amount
            };
            
            await _discountRepository.UpdateDiscount(updatedCoupon);
            _logger.LogInformation($"Coupon for the {request.ProductName} is successfully updated");
            var couponModel = _mapper.Map<CouponModel>(updatedCoupon);
            return couponModel;
        }
   }
}
