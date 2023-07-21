using System.Net;
using MoviePlatformApi.Configuration;
using MoviePlatformApi.Models;
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

                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(httpContext, ex, ConfigSetting, logger);
            }
        }
        private bool Validate()
        {
            var token = httpContext.Request.Headers["Authorization"].Count > 0 ? httpContext.Request.Headers["Authorization"][0] : "";
            if (string.IsNullOrEmpty(token))
            {
                httpContext.Response.StatusCode = (int)HttpStatusCode.Forbidden;
                throw new Exception("No token found on the request header");
            }
            if (token == ConfigSetting.DefaultToken)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception, Setting ConfigSetting, ILogger logger)
        {
            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var errorMsg = "";
            logger.LogError($" ErrorMsg = {exception.Message}:{exception?.InnerException?.Message} \n FullLog = {exception.StackTrace}");
            if (!ConfigSetting.IsProduction)
            {
                errorMsg = JsonConvert.SerializeObject(new Error()
                {
                    ErrorMsg = exception.Message + ":" + exception?.InnerException?.Message,
                    StatusCode = context.Response.StatusCode,
                    FullLog = exception.StackTrace
                });
            }
            else
            {
                errorMsg = JsonConvert.SerializeObject(new Error()
                {
                    ErrorMsg = "Server Error Please contact administrator.",
                    StatusCode = context.Response.StatusCode,
                    FullLog = "Server Error"
                });
            }
            return context.Response.WriteAsync(errorMsg);
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