using Catalog.Core.Entities;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Data.Context
{
    public class CatalogContext: ICatalogContext
    {
        public IMongoCollection<Product> Products { get; }
        public IMongoCollection<ProductBrand> Brands { get; }
        public IMongoCollection<ProductType> Types { get; }

        public CatalogContext(IConfiguration configuration)
        { 
            var mongoClient = new MongoClient(configuration["DatabaseSettings:ConnectionString"]);
            var mongoDatabase = mongoClient.GetDatabase(configuration["DatabaseSettings:DatabaseName"]);

            Products = mongoDatabase.GetCollection<Product>(configuration["DatabaseSettings:ProductsCollectionName"]);
            Brands = mongoDatabase.GetCollection<ProductBrand>(configuration["DatabaseSettings:BrandsCollectionName"]);
            Types = mongoDatabase.GetCollection<ProductType>(configuration["DatabaseSettings:TypesCollectionName"]);

            // seed data if collection is empty
            ProductContextSeed.SeedAsync(Products).Wait();
            BrandContextSeed.SeedAsync(Brands).Wait();
            TypeContextSeed.SeedAsync(Types).Wait();
        }
    }
}
