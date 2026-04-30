using AutoMapper;
using Catalog.Application.GRPCServices;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetAllProductsQueryHandler: IRequestHandler
        <GetAllProductsQuery, Pagination<ProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;
        private readonly DiscountGrpcService _discountGrpcService;

        public GetAllProductsQueryHandler(
            IMapper mapper, IProductRepository productRepository, DiscountGrpcService discountGrpcService)
        {
            _mapper = mapper;
            _productRepository = productRepository;
            _discountGrpcService = discountGrpcService;
        }

        public async Task<Pagination<ProductResponseDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            //var productsList = _productRepository.GetAllWithIncludesAsync(new List<string> { "ProductType", "ProductBrand" }).Result;
            var productsList = await _productRepository.GetAllProducts(request.SpecParams);
            foreach (var product in productsList.Data)
            {
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
            }

            var productsResponseDto = _mapper.Map<Pagination<ProductResponseDto>>(productsList);
            return productsResponseDto;
        }
    }
}
