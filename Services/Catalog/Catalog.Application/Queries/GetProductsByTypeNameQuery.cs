using Catalog.Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Queries
{
    public class GetProductsByTypeNameQuery: IRequest<IList<ProductResponseDto>>
    {
        public string Name { get; set; }
        public GetProductsByTypeNameQuery(string name)
        {
            Name = name;
        }
    }
}
