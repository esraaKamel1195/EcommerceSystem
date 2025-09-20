using Catalog.Application.Responses;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Application.Queries
{
    public class GetProductsByBrandNameQuery: IRequest<IList<ProductResponseDto>>
    {
        public string Name { get; set; }
        public GetProductsByBrandNameQuery(string name)
        {
            Name = name;
        }
    }
}
