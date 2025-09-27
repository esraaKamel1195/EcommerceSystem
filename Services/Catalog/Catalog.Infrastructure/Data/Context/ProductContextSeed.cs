using Catalog.Core.Entities;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data.Context
{
    public static class ProductContextSeed
    {
        public static async Task SeedAsync(IMongoCollection<Product> productCollection)
        {
            var hasProducts = await productCollection.Find(_ => true).AnyAsync();
            if (hasProducts)
            {
                return;
            }

            var basePath = AppContext.BaseDirectory;
            var filePath = Path.Combine(basePath, "Data", "SeedData", "products.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file for products not found: {filePath}");
                return;
            }
            var productsData = await File.ReadAllTextAsync(filePath);
            var products = JsonSerializer.Deserialize<IEnumerable<Product>>(productsData);
            if (products != null && products.Any())
            {
                await productCollection.InsertManyAsync(products);
            }
        }
    }
}
