using AutoMapper;
using Catalog.Application.GRPCServices;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly DiscountGrpcService _discountGrpcService;

        public GetProductByIdQueryHandler(
            IMapper mapper,
            IProductRepository productRepository,
            DiscountGrpcService discountGrpcService
        )
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _discountGrpcService = discountGrpcService;
        }

        public async Task<ProductResponseDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _productRepository.GetProductById(request.Id);
            var coupon = await _discountGrpcService.GetDiscount(product.Name);
            if (coupon != null)
            {
                product.HasDiscount = true;
                product.PriceAfterDiscount = product.Price - coupon.Amount;
                product.DiscountAmount = coupon.Amount;
            }
            else
            {
                product.HasDiscount = false;
                product.PriceAfterDiscount = null;
                product.DiscountAmount = 0;
            }
            var productResponse = _mapper.Map<ProductResponseDto>(product);
            return productResponse;
        }
    }
}
