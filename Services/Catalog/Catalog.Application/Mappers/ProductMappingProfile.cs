using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specs;

namespace Catalog.Application.Mappers
{
    public class ProductMappingProfile: Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<ProductBrand,BrandResponseDto>().ReverseMap();
            CreateMap<ProductType, TypeResponseDto>().ReverseMap();
            CreateMap<Product, ProductResponseDto>().ReverseMap();

            // Added mapping for the AutoMapper exception: CreateProductCommand -> Product
            CreateMap<CreateProductCommand, Product>();
            CreateMap<UpdateProductCommand, Product>();
            CreateMap<Pagination<Product>, Pagination<ProductResponseDto>>().ReverseMap();
        }
    }
}
