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
    public class UpdateProductCommandHandler : IRequestHandler<UpdateProductCommand, bool>
    {
        private readonly IMapper _mapper;
        private readonly IProductRepository _productRepository;

        public UpdateProductCommandHandler(IMapper mapper, IProductRepository productRepository)
        {
            _mapper = mapper;
            _productRepository = productRepository;
        }
        public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var productEntity = _mapper.Map<Product>(request);
            var updateProduct = await _productRepository.UpdateProduct(productEntity);
            return updateProduct;
        }

        //try other handle way
        /*public async Task<bool> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
        {
            var updateProduct = await _productRepository.UpdateProduct(new Product()
            {
                Name = request.Name,
                Description = request.Description,
                ImageFile = request.ImageFile,
                Brands = request.Brands,
                Types = request.Types,
                Summary = request.Summary,
                Price = request.Price,
            });

            return updateProduct;
        }*/
    }
}
