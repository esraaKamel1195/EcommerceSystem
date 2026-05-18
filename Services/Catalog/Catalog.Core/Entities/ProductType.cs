using MongoDB.Bson.Serialization.Attributes;

namespace Catalog.Core.Entities
{
    public class ProductType: BaseEntity
    {
        [BsonElement("name")] //for name field in MongoDB collection
        public string Name { get; set; }
    }
}
