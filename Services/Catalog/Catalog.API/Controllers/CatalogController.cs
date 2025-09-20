using Catalog.Application.Commands;
using Catalog.Application.Queries;
using Catalog.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Catalog.API.Controllers
{
    public class CatalogController : BaseApiController
    {
        private readonly IMediator _mediator;
        public CatalogController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("[action]/id", Name = "GetProductById")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<ProductResponseDto>> GetProductById(string id) 
        {
            GetProductByIdQuery query = new GetProductByIdQuery(id);
            ProductResponseDto sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]", Name = "GetAllProducts")]
        [ProducesResponseType(typeof(IList<ProductResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponseDto>>> GetAllProducts()
        {
            GetAllProductsQuery query = new GetAllProductsQuery();
            IList<ProductResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]", Name = "GetAllBrands")]
        [ProducesResponseType(typeof(IList<BrandResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<BrandResponseDto>>> GetAllBrands()
        {
            GetAllBrandsQuery query = new GetAllBrandsQuery();
            IList<BrandResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]", Name = "GetAllTypes")]
        [ProducesResponseType(typeof(IList<TypeResponseDto>), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<TypeResponseDto>>> GetAllTypes()
        {
            GetAllTypesQuery query = new GetAllTypesQuery();
            IList<TypeResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]/name", Name = "GetProductsByName")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponseDto>>> GetProductsByName(string name)
        {
            GetProductsByNameQuery query = new GetProductsByNameQuery(name);
            IList<ProductResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]/brandId", Name = "GetProductsByBrandId")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponseDto>>> GetProductsByBrandId(string id)
        {
            GetProductsByBrandIdQuery query = new GetProductsByBrandIdQuery(id);
            IList<ProductResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]/brandName", Name = "GetProductsByBrandName")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponseDto>>> GetProductsByBrandName(string name)
        {
            GetProductsByBrandNameQuery query = new GetProductsByBrandNameQuery(name);
            IList<ProductResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]/typeId", Name = "GetProductsByTypeId")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponseDto>>> GetProductsByTypeId(string id)
        {
            GetProductsByTypeIdQuery query = new GetProductsByTypeIdQuery(id);
            IList<ProductResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpGet]
        [Route("[action]/typeName", Name = "GetProductsByTypeName")]
        [ProducesResponseType(typeof(ProductResponseDto), (int)HttpStatusCode.OK)]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        public async Task<ActionResult<IList<ProductResponseDto>>> GetProductsByTypeName(string name)
        {
            GetProductsByTypeNameQuery query = new GetProductsByTypeNameQuery(name);
            IList<ProductResponseDto> sendQueryResult = await _mediator.Send(query);
            return Ok(sendQueryResult);
        }

        [HttpPost]
        [Route("CreateProduct")]
        public async Task<ActionResult<ProductResponseDto>> CreateProduct([FromBody] CreateProductCommand createProductCommand)
        {
            return Ok(await _mediator.Send<ProductResponseDto>(createProductCommand));
        }

        [HttpPut]
        [Route("UpdateProduct")]
        public async Task<ActionResult<bool>> UpdateProduct([FromBody] UpdateProductCommand updateProductCommand)
        {
            return Ok(await _mediator.Send<bool>(updateProductCommand));
        }

        [HttpDelete]
        [Route("DeleteProduct/id")]
        public async Task<ActionResult<bool>> DeleteProduct(string id)
        {
            var command = new DeleteProductCommand(id);
            return Ok(await _mediator.Send<bool>(command));
        }
    }
}
