using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Handlers.Commands
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductResponseDto>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public CreateProductCommandHandler(IMapper mapper, IProductRepository productRepository) {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<ProductResponseDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = _mapper.Map<Product>(request);
            //if (productEntity == null) {
                var newProduct = await _productRepository.CreateProduct(productEntity);
                var productResponse = _mapper.Map<ProductResponseDto>(newProduct);
                return productResponse;
            //}
        }
    }
}
