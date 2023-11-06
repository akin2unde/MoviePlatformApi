using MoviePlatformApi.ClassExtension;
using MoviePlatformApi.Configuration;
using MoviePlatformApi.Models;
using MoviePlatformApi.Services;
using MoviePlatformApi.Util;

namespace MoviePlatformApi.Services
{
    public class AuthService : BaseService<User>
    {
        public AuthService(Setting setting, IHttpContextAccessor context, User user, ILogger<AuthService> logger) : base(setting, context, user, logger)
        {
        }
        public new List<User> Save(List<User> values)
        {
            foreach (var item in values)
            {
                if (!item.MailAddress.Trim().IsMailValid())
                {
                    throw new AppException("Invalid Email Address");
                }
            }
            base.Save(values);
            values.ForEach(v =>
            {
                v.Password = "";
            });
            return values;
        }

        public PagedResult<User> Get(string param, string id, int skip, int limit)
        {
            PagedResult<User> result = new PagedResult<User>();
            if (string.IsNullOrEmpty(param) && string.IsNullOrEmpty(id))
            {
                result = base.Get(skip, limit);
            }
            else if (!string.IsNullOrEmpty(id))
            {
                result = base.Get(f => f.Id == id);
            }
            else if (!string.IsNullOrEmpty(param))
            {
                result = base.Search(param, skip, limit);
            }
            return result;
        }
        public static void SeedAppDefault(Setting setting1)
        {
            var u = new User() { IsSeeded = true };
            var authService = new AuthService(setting1, null, u, null);
            var found = authService.Get(0, 1);
            if (found.Result.Count() == 0)
            {
                u.Active = true;
                u.Password = "JhcSv4t6aa194XhsEiu7xU9GelY/WCkYK9RUi0rqBHw=";//Default
                u.Name = "App Default";
                u.MailAddress = "movieapi@gmail.com";
                u.PhoneNumber = "090";
                authService.Save(new List<User> { u });
                var token = JWT.GenerateJWT(u);
                Console.WriteLine(token);
            }

        }

    }

}