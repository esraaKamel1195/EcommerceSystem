using Discount.Core.Entities;

namespace Discount.Core.Repositories
{
    public interface IDiscountRepository
    {
        Task<Coupon> GetDiscount(string productName);
        Task<bool> CreateDiscount(Coupon copoun);
        Task<bool> UpdateDiscount(Coupon copoun);
        Task<bool> DeleteDiscount(string productName);
    }
}
