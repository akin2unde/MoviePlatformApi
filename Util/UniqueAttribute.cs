
namespace MoviePlatformApi.Models
{
    public class UniqueAttribute : System.Attribute
    {
        public int Position;
        public UniqueAttribute(int pos)
        {
            this.Position = pos;
        }
        public UniqueAttribute()
        {
            this.Position = 1;
        }
    }
}
