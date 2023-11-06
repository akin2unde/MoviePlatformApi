using Microsoft.AspNetCore.Http.Json;
using MoviePlatformApi;
using MoviePlatformApi.Configuration;
using MoviePlatformApi.Middleware;
using MoviePlatformApi.Models;
using MoviePlatformApi.Services;
using MoviePlatformApi.Util;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Configuration
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

var setting = builder.Configuration.GetSection("Setting").Get<Setting>();
builder.Services.AddControllers().AddJsonOptions(x => x.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter()));
builder.Services.AddControllers()
            .AddJsonOptions(opts => opts.JsonSerializerOptions.PropertyNamingPolicy = null);
User user = new User();
builder.Services.AddTransient<User>((s) =>
{
    return user;
});
builder.Services.AddSingleton<Setting>((s) =>
{
    return setting;
}
);
builder.Services.AddTransient<MovieService>();
builder.Services.AddTransient<AuditTrailService>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});
builder.Services.AddHttpContextAccessor();
var app = builder.Build();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseStaticFiles(new StaticFileOptions()
{
    ServeUnknownFileTypes = true,
    DefaultContentType = "text/plain",
    OnPrepareResponse = (context) =>
    {
        var headers = context.Context.Response.GetTypedHeaders();
        headers.CacheControl = new Microsoft.Net.Http.Headers.CacheControlHeaderValue
        {
            Public = true,
            MaxAge = TimeSpan.FromDays(30)
        };
    }
});
//validate
app.Use(async (httpContext, next) =>
{
    string controllerName = string.Empty;
    string actionName = string.Empty;
    var token = httpContext.Request.Headers["Authorization"].Count > 0 ? httpContext.Request.Headers["Authorization"][0] : "";
    using var loggerFactory = LoggerFactory.Create(loggingBuilder => loggingBuilder
        .SetMinimumLevel(LogLevel.Trace)
        .AddConsole());
    ILogger logger = loggerFactory.CreateLogger<Program>();
    var u = JWT.GetUserFromJWT(token);
    if (httpContext.Request.Path != "/")
    {
        controllerName = httpContext.Request.Path.ToString().Split("/")[1];
    }
    if (httpContext.Request.Path == "/" || httpContext.Request.Path.ToString().Contains("favicon") || httpContext.Request.Path.ToString().Contains("swagger") || httpContext.Request.Path == "/index.html" || u != null)
    {
        if (u != null)
        {
            user.SetUserFromJWT(u);
        }
        await next();
    }
    else
    {
        throw new AppException("Token not valid");
    }

});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{

    setting.IsProduction = false;
}
app.UseSwagger();
app.UseSwaggerUI(c =>
   {
       c.SwaggerEndpoint("./swagger/v1/swagger.json", "Movie API Doc");
       c.RoutePrefix = string.Empty;
   });
AuthService.SeedAppDefault(setting);
app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<InterceptorMiddleware>();
// app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();
