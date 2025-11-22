
using MediatR;

namespace Discount.Application.Commands
{
    public class DeleteDiscountCouponCommand: IRequest<bool>
    {
        public string ProductName { get; set; }
        public DeleteDiscountCouponCommand(string productName)
        {
            ProductName = productName;
        }
    }
}
