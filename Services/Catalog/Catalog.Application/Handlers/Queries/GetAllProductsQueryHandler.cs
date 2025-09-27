using AutoMapper;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Core.Specs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Handlers.Queries
{
    public class GetAllProductsQueryHandler: IRequestHandler<GetAllProductsQuery, Pagination<ProductResponseDto>>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public GetAllProductsQueryHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }

        public Task<Pagination<ProductResponseDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
        {
            //var productsList = _productRepository.GetAllWithIncludesAsync(new List<string> { "ProductType", "ProductBrand" }).Result;
            var productsList = _productRepository.GetAllProducts(request.SpecParams).Result;
            var productsResponseDto = _mapper.Map<Pagination<ProductResponseDto>>(productsList);
            return Task.FromResult(productsResponseDto);
        }
    }
}
