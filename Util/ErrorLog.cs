using MoviePlatformApi.Models;

namespace MoviePlatformApi.Util
{
    public class ErrorLog : BaseModel
    {
        public string RequestType { get; set; }
        public string FullLog { get; set; }
        public string Data { get; set; }
        public string Url { get; set; }
        public string Controller { get; set; }
        public string User { get; set; }
        public string Action { get; set; }
        [Unique(3)]
        public string Code { get; set; } = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
    }
}
