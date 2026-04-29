using Discount.Core.Entities;
using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Core.Entities
{
     public class Product: BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Summary { get; set; }
        public string ImageFile { get; set; }
        [BsonRepresentation(MongoDB.Bson.BsonType.Decimal128)]
        public decimal Price { get; set; }
        public decimal PriceAfterDiscount { get; set; }
        public ProductBrand Brands { get; set; }
        public ProductType Types { get; set; }
        public bool HasDiscount { get; set; } = false;
        public int DiscountAmount { get; set; } = 0;
        public Coupon? Coupon { get; set; } = null;
    }
}
