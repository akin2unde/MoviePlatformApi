using System.Text.Json.Serialization;
using MongoDB.Bson.Serialization.Attributes;

namespace MoviePlatformApi.Models
{
    public class File
    {
        public string? Extension { get; set; }
        public string Name { get; set; }
        public string? Url { get; set; }
        public string Data { get; set; }
        public int Position { get; set; } = 1;

        public DateTimeOffset CreatedAt { get; set; } = DateTime.Now;

        public DateTimeOffset UpdatedAt { get; set; } = DateTime.Now;

        public ObjectState State { get; set; } = ObjectState.New;
        [BsonIgnore]
        [JsonIgnore]
        public byte[]? Attachment { get; set; }

    }
}
