using Asp.Versioning;
using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Application.Responses;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.Api.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    public class BasketApiController : BaseApiController
    {
        private readonly IMediator _mediator;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IMapper _mapper;
        private readonly ILogger<BasketApiController> _logger;

        public BasketApiController(
            IMediator mediator,
            IPublishEndpoint publishEndpoint,
            IMapper mapper,
            ILogger<BasketApiController> logger
        )
        {
            _mediator = mediator;
            _publishEndpoint = publishEndpoint;
            _mapper = mapper;
            _logger = logger;
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
            DeleteBasketByUserNameCommand command = new DeleteBasketByUserNameCommand(userName);
            return Ok( await _mediator.Send(command));
        }

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckout basketCheckout)
        {
            //get basket by username
            var query = new GetBasketByUserNameQuery(basketCheckout.Username);
            var basket = await _mediator.Send(query);

            if (basket == null)
            { 
                return BadRequest();  
            }

            var eventMsg = _mapper.Map<BasketCheckoutEvent>(basketCheckout);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);

            _logger.LogInformation($"Basket published for {basket.UserName} with V1 endpoint");

            //remove from basket
            var deletedCMD = new DeleteBasketByUserNameCommand(basket.UserName);
            await _mediator.Send(deletedCMD);
            return Accepted();
        }
    }
}