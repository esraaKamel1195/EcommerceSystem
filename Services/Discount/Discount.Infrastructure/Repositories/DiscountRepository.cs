using Basket.Application.GRPCServices;
using Dapper;
using Discount.Core.Entities;
using Discount.Core.Repositories;
using Microsoft.Extensions.Configuration;

namespace Discount.Infrastructure.Repositories
{
    public class DiscountRepository : IDiscountRepository
    {
        private readonly IConfiguration _configuration;
        public DiscountRepository(IConfiguration configuration, DiscountGrpcService discountGrpcService)
        {
            _configuration = configuration;
        }
        public async Task<Coupon> GetDiscount(string productName)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var copoun = await connection.QueryFirstOrDefaultAsync<Coupon>("Select * From Coupon where ProductName = @ProductName", 
                new
                { productName = productName }
               );
            if (copoun == null)
            {
                return new Coupon
                { Amount = 0, Description = "No Discount Available for this product", ProductName = "No discount" };
            }
            return copoun;
        }
        public async Task<bool> CreateDiscount(Coupon copoun)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync
                ("INSERT INTO Coupon (ProductName, Description, Amount) VALUES (@ProductName, @Description, @Amount",
                new
                {
                    ProductName = copoun.ProductName,
                    Description = copoun.Description,
                    Amount = copoun.Amount,
                });
            if (affected == 0)
            { return false; }
            return true;
        }
        public async Task<bool> UpdateDiscount(Coupon copoun)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync
                ("UPDATE Coupon SET ProductName = @ProductName, Description = @Description, Amount = @Amount where Id = @Id", 
                new 
                {
                    ProductName = copoun.ProductName,
                    Description = copoun.Description,
                    Amount = copoun.Amount,
                    Id = copoun.Id,
                });
            if(affected == 0)
            {  return false; }
            return true;
        }
        public async Task<bool> DeleteDiscount(string ProductName)
        {
            await using var connection = new Npgsql.NpgsqlConnection(_configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
            var affected = await connection.ExecuteAsync
                ("DELETE FROM coupon WHERE ProductName = @ProductName",
                new
                {
                    ProductName = @ProductName,
                });
            if (affected == 0)
            { return false; }
            return true;
        }
    }
}
