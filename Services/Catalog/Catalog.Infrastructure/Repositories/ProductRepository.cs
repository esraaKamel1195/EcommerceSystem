using Catalog.Core.Entities;
using Catalog.Core.Repositories;
using Catalog.Infrastructure.Data.Context;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository : IProductRepository, IBrandRepository, ITypeRepository
    {
        public ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }
        public async Task<Product> GetProductById(string id)
        {
            return await _context.Products.Find(p => p.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            return await _context.Products.Find(p => p.Name == name).ToListAsync();
        }
        public async Task<Product> CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
            return product;
        }
        public async Task<bool> UpdateProduct(Product product)
        {
            var UpdatedProduct = await _context.Products.ReplaceOneAsync(p => p.Id == product.Id, product);
            return UpdatedProduct.IsAcknowledged && UpdatedProduct.ModifiedCount > 0;
        }
        public async Task<bool> DeleteProduct(string id)
        {
            var deletedProduct = await _context.Products.DeleteOneAsync(p => p.Id == id);
            return deletedProduct.IsAcknowledged && deletedProduct.DeletedCount > 0;
        }

        public async Task<IEnumerable<ProductBrand>> GetAllBrands()
        {
            return await _context.Brands.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetAllProducts()
        {
            return await _context.Products.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<ProductType>> GetAllTypes()
        {
            return await _context.Types.Find(_ => true).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandId(string brandId)
        {
            return await _context.Products.Find(p => p.Brands.Id == brandId).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByBrandName(string brandName)
        {
            return await _context.Products.Find(p => p.Brands.Name == brandName).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByTypeId(string typeId)
        {
            return await _context.Products.Find(p => p.Types.Id == typeId).ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByTypeName(string typeName)
        {
            return await _context.Products.Find(p => p.Types.Name == typeName).ToListAsync();
        }
    }
}
