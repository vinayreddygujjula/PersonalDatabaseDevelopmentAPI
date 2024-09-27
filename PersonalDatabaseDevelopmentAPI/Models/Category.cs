using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace PersonalDatabaseDevelopmentAPI.Models
{
    public class Category
    {
        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("user_id")]
        public string UserId { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }
    }
}