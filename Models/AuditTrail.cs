using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoviePlatformApi.Models
{
    public class AuditTrail : BaseModel
    {
        [BsonRepresentation(BsonType.ObjectId)]
        [Unique]
        public string UniqueId { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
        public string Name { get; set; }
        public string MailAddress { get; set; }
        public string Entity { get; set; }
        public string EntityId { get; set; }
    }
}
