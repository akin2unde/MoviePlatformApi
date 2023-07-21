

using MoviePlatformApi.Configuration;

namespace MoviePlatformApi.Services
{
    public class MovieService : BaseService
    {
        public MovieService(Setting setting, IHttpContextAccessor context) : base(setting, context)
        {

        }
    }
}