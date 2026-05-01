using Asp.Versioning;
using AutoMapper;
using Basket.Application.Commands;
using Basket.Application.Queries;
using Basket.Core.Entities;
using EventBus.Messages.Events;
using MassTransit;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Basket.Api.Controllers.V2
{
    [ApiVersion("2")]
    [Route("api/v{Version:apiVersion}/[controller]")]
    [ApiController]
    public class BasketApiController : ControllerBase
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

        [Route("[action]")]
        [HttpPost]
        [ProducesResponseType((int)HttpStatusCode.Accepted)]
        [ProducesResponseType((int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Checkout([FromBody] BasketCheckoutV2 basketCheckout)
        {
            //get basket by username
            var query = new GetBasketByUserNameQuery(basketCheckout.Username);
            var basket = await _mediator.Send(query);

            if (basket == null)
            {
                return BadRequest();
            }

            var eventMsg = _mapper.Map<BasketCheckoutEventV2>(basket);
            eventMsg.TotalPrice = basket.TotalPrice;
            await _publishEndpoint.Publish(eventMsg);

            _logger.LogInformation($"Basket published for {basket.UserName}");

            //remove from basket
            var deletedCMD = new DeleteBasketByUserNameCommand(basket.UserName);
            await _mediator.Send(deletedCMD);
            return Accepted();
        }
    }
}
