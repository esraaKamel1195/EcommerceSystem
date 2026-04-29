using Dapper;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscountRepository> _logger;

        public DiscountRepository(
            IConfiguration configuration,
            ILogger<DiscountRepository> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        private Npgsql.NpgsqlConnection GetConnection() =>
            new Npgsql.NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString"));

        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var connection = GetConnection();
            var copoun = await connection.QueryFirstOrDefaultAsync<Coupon>(
                "SELECT * FROM Coupon WHERE ProductName = @ProductName", 
                new
                { ProductName = productName }
               );

            _logger.LogInformation($"Discount for the {productName} trying to retrieved from database {copoun}");

            //if (copoun == null)
            //{
            //    return new Coupon
            //    { Amount = 0, Description = "No Discount Available for this product", ProductName = "No discount" };
            //}
            return copoun;
        }

        public async Task<Coupon> CreateDiscount(Coupon copoun)
        {
            await using var connection = GetConnection();
            var affected = await connection.ExecuteAsync
                ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount)",
                new
                {
                    ProductName = copoun.ProductName,
                    Description = copoun.Description,
                    Amount = copoun.Amount,
                });
            //if (affected == 0)
            //{ return  }

            Task<Coupon> createdCoupon = GetDiscount(copoun.ProductName);
            return createdCoupon.Result;
        }

        public async Task<Coupon> UpdateDiscount(Coupon copoun)
        {
            await using var connection = GetConnection();
            var affected = await connection.ExecuteAsync
                ("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount where Id = @Id", 
                new 
                {
                    ProductName = copoun.ProductName,
                    Description = copoun.Description,
                    Amount = copoun.Amount,
                    Id = copoun.Id,
                });
            //if(affected == 0)
            //{  return false; }
            Task<Coupon> createdCoupon = GetDiscount(copoun.ProductName);
            return createdCoupon.Result;
        }
        public async Task<bool> DeleteDiscount(string productName)
        {
            await using var connection = new Npgsql.NpgsqlConnection(
                _configuration.GetValue<string>("DatabaseSettings:ConnectionString")
            );
            var affected = await connection.ExecuteAsync
                ("DELETE FROM coupon WHERE ProductName = @ProductName",
                new
                {
                    ProductName = productName,
                });
            if (affected == 0)
            { return false; }
            return true;
        }
    }
}