using Discount.Core.Entities;
using Discount.Core.Repositories;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        public async Task<bool> CreateDiscount(Copoun copoun)
        {
            throw new NotImplementedException();
        }

        public async Task DeleteDiscount(string productName)
        {
            throw new NotImplementedException();
        }

        public async Task<Copoun> GetDiscount(string productName)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateDiscount(Copoun copoun)
        {
            throw new NotImplementedException();
        }
    }
}
