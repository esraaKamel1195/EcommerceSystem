using Discount.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Discount.Core.Repositories
{
    public interface IDiscountRepository
    {
        Task<Copoun> GetDiscount(string productName);
        Task<bool> CreateDiscount(Copoun copoun);
        Task<bool> UpdateDiscount(Copoun copoun);
        Task DeleteDiscount(string productName);
    }
}
