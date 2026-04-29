using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.GRPCServices;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Discount.Grpc.Protos;
using MediatR;

namespace Catalog.Application.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly DiscountGrpcService _discountGrpcService;

        public CreateProductCommandHandler(
            IMapper mapper,
            IProductRepository productRepository,
            DiscountGrpcService discountGrpcService
        ) {
           _mapper = mapper;
           _productRepository = productRepository;
           _discountGrpcService = discountGrpcService;
        }

        public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.HasDiscount && request.DiscountAmount != 0)
            {
                await _discountGrpcService.CreateDiscount(new CouponModel
                {
                    ProductName = request.Name,
                    Description = "discount",
                    Amount = request.DiscountAmount
                });

                var coupon = await _discountGrpcService.GetDiscount(request.Name);

                if (coupon is not null && coupon.Amount == request.DiscountAmount)
                {
                    request.PriceAfterDiscount = request.Price - coupon.Amount;
                    request.DiscountAmount = coupon.Amount;
                }
            }

            var productEntity = _mapper.Map<Product>(request);
            //if (productEntity == null) {
                var newProduct = await _productRepository.CreateProduct(productEntity);
                var productResponse = _mapper.Map<ProductResponseDto>(newProduct);
                return productResponse;
            //}
        }
    }
}
