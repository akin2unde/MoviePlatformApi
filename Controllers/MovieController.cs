using System.Net;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformApi.Models;
using MoviePlatformApi.Services;

namespace MoviePlatformApi.Controllers;

[Route("[controller]")]
[ApiController]
public class MovieController : ControllerBase
{
    private readonly ILogger<MovieController> _logger;
    private MovieService Service;
    public MovieController(ILogger<MovieController> logger, MovieService service)
    {
        _logger = logger;
        Service = service;
    }

    [HttpGet("GetAll/{skip?}/{limit?}")]
    public IActionResult GetAll(int skip, int limit)
    {
        var result = Service.Get(skip, limit);
        return Ok(result); ;
    }

    [HttpGet("GetWithId/{param:length(24)}")]
    public IActionResult GetWithId(string param)
    {
        var result = Service.Get(a => a.Id == param);
        return Ok(result);
    }
    [HttpPost("Save")]
    public IActionResult Post([FromBody] List<Movie> values)
    {
        if (values != null)
        {
            var result = Service.Save(values);
            return Ok(result);
        }
        return NotFound(new Error { ErrorMsg = "No data recieved", StatusCode = (int)HttpStatusCode.Forbidden });
    }

    [HttpDelete("Delete")]
    public IActionResult Delete([FromQuery] string ids)
    {
        if (string.IsNullOrWhiteSpace(ids))
        {
            return NotFound(new Error { ErrorMsg = "Server didnt recieve any delete data", StatusCode = (int)HttpStatusCode.Forbidden });
        }
        Service.Remove(ids);
        return NoContent();
    }

    [HttpGet("Search/{param}/{skip?}/{limit?}")]
    public ActionResult<List<Movie>> Search(string param, int skip, int limit) => Ok(Service.Search(param, skip, limit));

}
