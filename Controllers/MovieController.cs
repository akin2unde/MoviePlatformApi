using Microsoft.AspNetCore.Mvc;
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

    [HttpGet("GetMovieByTitle/{searchStr}")]
    public async Task<IActionResult> GetPlayers(string searchStr)
    {
        var res = await Service.Get(searchStr);
        return Ok(res);
    }
}
