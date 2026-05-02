using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;

namespace Catalog.Application.Handlers.Queries
{
    public class GetProductsByTypeNameQueryHandler : IRequestHandler<GetProductsByTypeNameQuery, IList<ProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetProductsByTypeNameQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public Task<IList<ProductResponseDto>> Handle(GetProductsByTypeNameQuery request, CancellationToken cancellationToken)
        {
            var productsList = _productRepository.GetProductsByTypeName(request.Name).Result;
            var productsResponseDto = _mapper.Map<IList<ProductResponseDto>>(productsList.ToList());
            return Task.FromResult(productsResponseDto);
        }
    }
}