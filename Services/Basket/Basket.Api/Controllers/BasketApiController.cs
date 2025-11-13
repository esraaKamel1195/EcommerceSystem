using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.Api.Controllers
{
    public class BasketApiController : BaseApiController
    {
        private readonly IMediator _mediator;

        public BasketApiController(IMediator mediator) 
        {
            _mediator = mediator;
        }

        [HttpGet]
        [Route("[action]/{userName}", Name="GetBasketByUsername")]
        [ProducesResponseType(typeof(ShoppingCartResponse),(int)(HttpStatusCode.OK))]
        public async Task<ActionResult<ShoppingCartResponse>> GetBasket(string userName)
        {
            GetBasketByUserNameQuery query = new GetBasketByUserNameQuery(userName);
            ShoppingCartResponse sendQuery = await _mediator.Send(query);
            return Ok(sendQuery);
        }

        [HttpPost("CreateBasket")]
        [ProducesResponseType(typeof(ShoppingCartResponse),(int)(HttpStatusCode.OK))]
        public async Task<ActionResult<ShoppingCartResponse>> UpdateBasket([FromBody] CreateShoppingCartCommand createCommand)
        {
            ShoppingCartResponse basket = await _mediator.Send(createCommand);
            return Ok(basket);
        }

        [HttpDelete]
        [Route("[action]/{userName}", Name="DeleteBasketByUserName")]
        [ProducesResponseType(typeof(Unit),(int)(HttpStatusCode.OK))]
        public async Task<ActionResult<Unit>> DeleteBasket(string userName)
        {
            DeleteBaskerByUserNameCommand command = new DeleteBaskerByUserNameCommand(userName);
            return Ok( await _mediator.Send(command));
        }
    }
}
