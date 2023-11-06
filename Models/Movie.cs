
namespace MoviePlatformApi.Models
{

    public class Movie : BaseModel
    {
        [Unique]
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Rating { get; set; }
        public decimal TicketPrice { get; set; }
        public List<string> Genre { get; set; } = new List<string>();
        public string Country { get; set; }
        public string Photo { get; set; }
        public File? PhotoFile { get; set; }
    }

}