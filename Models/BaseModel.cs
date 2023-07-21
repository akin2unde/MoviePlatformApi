

namespace MoviePlatformApi.Models
{

    public class BaseModel
    {
        public string Id { get; set; }
        public ObjectState State { get; set; } = ObjectState.New;

    }
}