using Asp.Versioning;
using Discount.Application.Commands;
using Discount.Application.Queries;
using Discount.Core.Entities;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Discount.API.Controllers
{
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/[controller]")]
    [ApiController]
    public class DiscountController : ControllerBase
    {
        private readonly IMediator _mediator;
        public DiscountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("{productName}", Name = "GetDiscount")]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        public async Task<ActionResult<Coupon>> GetDiscount(string productName)
        {
            var query = new GetDiscountQuery(productName);
            var coupon = await _mediator.Send(query);
            return Ok(coupon);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        public async Task<ActionResult<Coupon>> CreateDiscount([FromBody] Coupon coupon)
        {
            var command = new CreateDiscountCouponCommand
            {
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount
            };
            var createdCoupon = await _mediator.Send(command);
            return CreatedAtRoute("GetDiscount", new { productName = createdCoupon.ProductName }, createdCoupon);
        }

        [HttpPut]
        [ProducesResponseType(typeof(Coupon), StatusCodes.Status200OK)]
        public async Task<ActionResult<Coupon>> UpdateDiscount([FromBody] Coupon coupon)
        {
            var command = new UpdateDiscountCouponCommand
            {
                Id = coupon.Id,
                ProductName = coupon.ProductName,
                Description = coupon.Description,
                Amount = coupon.Amount,
            };
            var updatedCoupon = await _mediator.Send(command);
            return Ok(updatedCoupon);
        }


        [HttpDelete("{productName}")]
        [ProducesResponseType(typeof(void), StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteDiscount(string productName)
        {
            var command = new DeleteDiscountCouponCommand(productName);
            var result = await _mediator.Send(command);
            if (result)
            {
                return NoContent();
            }
            return NotFound();
        }
    }
}
