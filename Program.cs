using Microsoft.AspNetCore.Http.Json;
using MoviePlatformApi.Configuration;
using MoviePlatformApi.Middleware;
using MoviePlatformApi.Services;

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
builder.Services.AddSingleton<Setting>((s) =>
{
    return setting;
});
builder.Services.AddTransient<MovieService>();
builder.Services.AddHttpContextAccessor();
builder.Services.Configure<JsonOptions>(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = null;
});
var app = builder.Build();
app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    setting.IsProduction = false;
}

// app.UseHttpsRedirection();
app.UseInterceptorMiddleware();


app.UseAuthorization();

app.MapControllers();

app.Run();
