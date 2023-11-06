

using MoviePlatformApi.Configuration;
using MoviePlatformApi.Models;
using MoviePlatformApi.Util;

namespace MoviePlatformApi.Services
{
    public class MovieService : BaseService<Movie>
    {
        public MovieService(Setting setting, IHttpContextAccessor context, User user) : base(setting, context, user)
        {

        }

        public new List<Movie> Save(List<Movie> movies)
        {
            foreach (var item in movies)
            {
                if (item.Rating < 1 || item.Rating > 5)
                {
                    throw new AppException("Rating must be between 1 to 5");
                }
                if (item.PhotoFile != null)
                {
                    SaveFile(item.PhotoFile, "", new string[] { item.Name });
                }
            }
            var res = base.Save(movies);
            return res;
        }
    }
}