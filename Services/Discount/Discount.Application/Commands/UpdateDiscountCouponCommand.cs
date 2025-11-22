using Discunt.Grpc.Protos;
using MediatR;

namespace Discount.Application.Commands
{
    // Define the command class that implements IRequest<CouponModel>
    public class UpdateDiscountCouponCommand : IRequest<CouponModel>
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
    }
}
