using Catalog.Application.Responses;
using Catalog.Core.Specs;
using MediatR;

namespace Catalog.Application.Queries
{
    public class GetAllProductsQuery: IRequest<Pagination<ProductResponseDto>>
    {
        public CatalogSpecParams SpecParams { get; set; }

        public GetAllProductsQuery(CatalogSpecParams specParams)
        {
            SpecParams = specParams;
        }
    }
}
