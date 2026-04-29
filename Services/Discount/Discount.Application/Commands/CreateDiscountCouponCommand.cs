using Discount.Core.Entities;
using Discount.Grpc.Protos;
using MediatR;

namespace Discount.Application.Commands
{
    public class CreateDiscountCouponCommand: IRequest<CouponModel>
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }

        //public CreateDiscountCouponCommand(Coupon coupon)
        //{
        //    ProductName = coupon.ProductName;
        //    Description = coupon.Description;
        //    Amount = coupon.Amount;
        //}
    }
}
