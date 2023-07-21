

using Newtonsoft.Json;

namespace MoviePlatformApi.Models
{

    public class Movie
    {
        public string Title { get; set; }
        public string Year { get; set; }
        public string Rated { get; set; }

        public string Released { get; set; }

        public string Runtime { get; set; }

        public string Genre { get; set; }

        public string Director { get; set; }

        public string Writer { get; set; }

        public string Actors { get; set; }

        public string Plots { get; set; }
        public string Language { get; set; }
        public string Country { get; set; }
        public string Awards { get; set; }
        public string Poster { get; set; }
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public string Metascore { get; set; }
        [JsonProperty("imdbRating")]
        public string imdbRating { get; set; }
        [JsonProperty("imdbVotes")]
        public string imdbVotes { get; set; }
        [JsonProperty("imdbID")]
        public string imdbID { get; set; }
        public string Type { get; set; }
        public string DVD { get; set; }
        public string BoxOffice { get; set; }
        public string Production { get; set; }
        public string Website { get; set; }
        public string Response { get; set; }

    }

}