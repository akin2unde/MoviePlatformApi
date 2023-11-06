using System.Net;
using MoviePlatformApi.Configuration;
using MoviePlatformApi.Models;
using MoviePlatformApi.Util;
using Newtonsoft.Json;

namespace MoviePlatformApi.Middleware
{
    public class InterceptorMiddleware
    {
        private Setting ConfigSetting;
        private readonly RequestDelegate _next;
        private HttpContext httpContext;
        private readonly ILogger logger;
        public InterceptorMiddleware(RequestDelegate next, Setting setting, ILogger<InterceptorMiddleware> logger)
        {
            ConfigSetting = setting;
            _next = next;
            this.logger = logger;
        }
        public async Task InvokeAsync(HttpContext httpContext)
        {
            this.httpContext = httpContext;
            try
            {
                if (httpContext.Request.Path == "/" || Validate())
                    await _next(httpContext);
                else
                {
                    throw new AppException("Invalid call");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private bool Validate()
        {
            var token = httpContext.Request.Headers["Authorization"].Count > 0 ? httpContext.Request.Headers["Authorization"][0] : "";
            if (string.IsNullOrEmpty(token))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                throw new AppException("No token found on the request header");
            }
            var user = JWT.GetUserFromJWT(token);
            if (user!=null && user.ExpireOn >= DateTime.Now)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public static class CustomInterceptorMiddleware
    {
        public static IApplicationBuilder UseInterceptorMiddleware(this IApplicationBuilder app)
        {
            return app.UseMiddleware<InterceptorMiddleware>();
        }
    }
}