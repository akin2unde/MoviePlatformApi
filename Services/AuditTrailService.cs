

using MoviePlatformApi.Configuration;
using MoviePlatformApi.Models;
using MoviePlatformApi.Util;

namespace MoviePlatformApi.Services
{
    public class AuditTrailService : BaseService<AuditTrail>
    {
        public AuditTrailService(Setting setting, IHttpContextAccessor context, User user) : base(setting, context, user)
        {

        }

    }
}