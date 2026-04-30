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
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, ProductResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly DiscountGrpcService _discountGrpcService;


        public UpdateProductCommandHandler(
            IMapper mapper, 
            IProductRepository productRepository,
            DiscountGrpcService discountGrpcService
        )
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _discountGrpcService = discountGrpcService;
        }

        public async Task<ProductResponseDto> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            if (request.HasDiscount && request.DiscountAmount != 0)
            {
                var coupon = await _discountGrpcService.GetDiscount(request.Name);
                if (coupon == null)
                {
                    await _discountGrpcService.CreateDiscount(new CouponModel
                    {
                        ProductName = request.Name,
                        Description = "discount",
                        Amount = request.DiscountAmount
                    });
                    coupon = await _discountGrpcService.GetDiscount(request.Name);
                }
                else if (coupon is not null)
                {
                    await _discountGrpcService.UpdateDiscount(new CouponModel
                    {
                        ProductName = request.Name,
                        Description = "discount",
                        Amount = request.DiscountAmount
                    });
                }

                request.PriceAfterDiscount = request.Price - coupon.Amount;
                request.DiscountAmount = coupon.Amount;
            }

            Product productEntity = _mapper.Map<Product>(request);
            Product updateProduct = await _productRepository.UpdateProduct(productEntity);
            ProductResponseDto newProduct = _mapper.Map<ProductResponseDto>(updateProduct);
            return newProduct;
        }
    }
}
