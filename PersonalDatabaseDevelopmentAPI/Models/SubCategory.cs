using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace PersonalDatabaseDevelopmentAPI.Models
{
    public class SubCategory
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("category_id")]
        public ObjectId CategoryId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}