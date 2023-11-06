

using System.Text.Json.Serialization;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MoviePlatformApi.Models
{

    public class BaseModel
    {

        [BsonRepresentation(BsonType.ObjectId)]
        public virtual string Id { get { return _id; } set { _id = value.IsNullOrEmpty() ? MongoDB.Bson.ObjectId.GenerateNewId().ToString() : value; } }
        [BsonIgnore]
        [JsonIgnore]
        private string? _id { get; set; }
        [BsonIgnore]
        public string? Error { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        [BsonIgnore]

        public ObjectState State { get; set; } = ObjectState.New;


        public string GetTableName()
        {
            return this.GetType().Name;
        }
    }
}