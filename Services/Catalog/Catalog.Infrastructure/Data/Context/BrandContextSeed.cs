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
    public static class BrandContextSeed
    {
        /*public static async Task SeedAsync(BrandContext brandContext)
        {
            if (!brandContext.ProductBrands.Any())
            {
                brandContext.ProductBrands.InsertMany(GetPreconfiguredProductBrands());
            }
            if (!brandContext.ProductTypes.Any())
            {
                brandContext.ProductTypes.InsertMany(GetPreconfiguredProductTypes());
            }
        }*/

        // try to seed data if collection is empty - for MongoDB implementation with other way
        public static async Task SeedAsync(IMongoCollection<ProductBrand> brandCollection) 
        {
            var hasBrands = await brandCollection.Find(_ => true).AnyAsync();
            if (hasBrands)
            {
                return;
            }
            var filePath = Path.Combine("Data", "SeedData", "brands.json");
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Seed file for brands not found: {filePath}");
                return;
            }
            var brandsData = await File.ReadAllTextAsync(filePath);
            var brands = JsonSerializer.Deserialize<IEnumerable<ProductBrand>>(brandsData);
            if (brands != null && brands.Any())
            {
                await brandCollection.InsertManyAsync(brands);
            }
        }
    }
}
