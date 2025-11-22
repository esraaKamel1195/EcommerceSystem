using AutoMapper;
using Discount.Core.Entities;
using Discunt.Grpc.Protos;

namespace Discount.Application.Mappers
{
    public class DiscountProfile: Profile
    {
        public DiscountProfile()
        {
            // Create your mapping configurations here
            CreateMap<Coupon, CouponModel>().ReverseMap();
        }
    }
}
