using AutoMapper;
using Catalog.Application.Commands;
using Catalog.Application.Responses;
using Catalog.Core.Entities;
using Catalog.Core.Specs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            CreateMap<Pagination<Product>, Pagination<ProductResponseDto>>().ReverseMap();
        }
    }
}
