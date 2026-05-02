using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Repositories;
using MediatR;


namespace Catalog.Application.Handlers.Queries
{
    public class GetProductsByBrandIdQueryHandler : IRequestHandler<GetProductsByBrandIdQuery, IList<ProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetProductsByBrandIdQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public Task<IList<ProductResponseDto>> Handle(GetProductsByBrandIdQuery request, CancellationToken cancellationToken)
        {
            var productsList = _productRepository.GetProductsByBrandId(request.Id).Result;
            var productsResponseDto = _mapper.Map<IList<ProductResponseDto>>(productsList.ToList());
            return Task.FromResult(productsResponseDto);
        }
    }
}
