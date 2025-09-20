using Catalog.Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Queries
{
    public class GetProductsByBrandIdQuery: IRequest<IList<ProductResponseDto>>
    {
        public string Id { get; set; }
        public GetProductsByBrandIdQuery(string id)
        {
            Id = id;
        }
    }
}
