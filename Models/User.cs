using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using JsonIgnoreAttribute = System.Text.Json.Serialization.JsonIgnoreAttribute;

namespace MoviePlatformApi.Models
{
    public class User : BaseModel
    {
        public string? PhoneNumber { get; set; }
        public string Name { get; set; }
        [Unique(pos: 1)]
        public string MailAddress { get; set; }
        public string Password { get; set; }
        public bool? Active { get; set; } = true;
        public bool? IsSeeded { get; set; } = true;
        public DateTime ExpireOn { get; set; } = DateTime.Now;


        public void SetUserFromJWT(User user)
        {
            this.Id = user.Id;
            this.MailAddress = user.MailAddress;
            this.Name = user.Name;
            this.ExpireOn = user.ExpireOn;
        }
    }
}
