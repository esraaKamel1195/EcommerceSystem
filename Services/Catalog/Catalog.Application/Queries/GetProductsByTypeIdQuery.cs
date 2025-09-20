using Catalog.Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Queries
{
    public class GetProductsByTypeIdQuery: IRequest<IList<ProductResponseDto>>
    {
        public string Id { get; set; }
        public GetProductsByTypeIdQuery(string id)
        {
            Id = id;
        }
    }
}
