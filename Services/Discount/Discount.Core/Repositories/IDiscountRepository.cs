using Discount.Core.Entities;

namespace Discount.Core.Repositories
{
    public interface IDiscountRepository
    {
        Task<Coupon> GetDiscount(string productName);
        Task<Coupon> CreateDiscount(Coupon copoun);
        Task<Coupon> UpdateDiscount(Coupon copoun);
        Task<bool> DeleteDiscount(string productName);
    }
}
