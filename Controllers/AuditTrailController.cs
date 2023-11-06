using System.Net;
using Microsoft.AspNetCore.Mvc;
using MoviePlatformApi.Models;
using MoviePlatformApi.Services;

namespace MoviePlatformApi.Controllers;

[Route("[controller]")]
[ApiController]
public class AuditTrailController : ControllerBase
{
    private readonly ILogger<MovieController> _logger;
    private AuditTrailService Service;
    public AuditTrailController(ILogger<MovieController> logger, AuditTrailService service)
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

    [HttpGet("Search/{param}/{skip?}/{limit?}")]
    public ActionResult<List<AuditTrail>> Search(string param, int skip, int limit) => Ok(Service.Search(param, skip, limit));

}
