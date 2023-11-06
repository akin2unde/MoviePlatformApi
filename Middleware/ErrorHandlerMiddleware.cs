using System.Net;
using System.Text.Json;
using MoviePlatformApi.Configuration;
using MoviePlatformApi.Models;
using MoviePlatformApi.Services;

namespace MoviePlatformApi.Util
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger logger;
        public ErrorHandlerMiddleware(RequestDelegate next, ILogger<ErrorHandlerMiddleware> logger)
        {
            this.logger = logger;
            _next = next;
        }
        private string GetBodyFromRequest(HttpContext context)
        {
            try
            {
                var body = new StreamReader(context.Request.Body);
                //The modelbinder has already read the stream and need to reset the stream index
                body.BaseStream.Seek(0, SeekOrigin.Begin);
                var requestBody = body.ReadToEnd();
                return requestBody;
            }
            catch (Exception)
            {
                return "";
            }
        }
        public async Task Invoke(HttpContext context, Setting ConfigSetting, User user)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string controllerName = context.Request.Path.ToString().Split("/")[1];
                string actionName = context.Request.Path.ToString().Split("/").Length > 2 ? context.Request.Path.ToString().Split("/")[2] : "";
                switch (error)
                {
                    case AppException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        break;
                    case KeyNotFoundException e:
                        // not found error
                        response.StatusCode = (int)HttpStatusCode.NotFound;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        break;
                }

                logger.LogError($" ErrorMsg = {error.Message}:{error?.InnerException?.Message} \n FullLog = {error.StackTrace}");
                string? errorMsg;
                if (!ConfigSetting.IsProduction || error is AppException)
                {
                    errorMsg = JsonSerializer.Serialize(new Error()
                    {
                        ErrorMsg = error.Message,
                        StatusCode = response.StatusCode,
                        FullLog = error.StackTrace
                    });
                }
                else
                {
                    errorMsg = JsonSerializer.Serialize(new Error()
                    {
                        ErrorMsg = "Server Error Please contact administrator.",
                        StatusCode = response.StatusCode,
                        FullLog = "Server Error"
                    });
                }
                //saveLog
                var logService = new BaseService<ErrorLog>(ConfigSetting, new HttpContextAccessor { HttpContext = context }, user);
                var data = new List<ErrorLog>{  new (){
                    Controller= controllerName,
                    Action= actionName,
                    User= user!=null? user.MailAddress:"",
                    FullLog= error.Message + ":" + error.StackTrace,
                    Url= context?.Request?.Path.ToString(),
                    RequestType =context?.Request?.Method,
                    Code=MongoDB.Bson.ObjectId.GenerateNewId().ToString(),
                    Data= actionName== "Login"?"":context.Request.Method=="POST"||context.Request.Method=="PUT"?  GetBodyFromRequest(context):""
                }};
                try
                {
                    logService.Save(
                        data
                    );
                }
                catch { }
                var result = JsonSerializer.Serialize(new Error { ErrorMsg = error?.Message, FullLog = error?.StackTrace, StatusCode = response.StatusCode });
                await response.WriteAsync(result);
            }
        }
    }
}