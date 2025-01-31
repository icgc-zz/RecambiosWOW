using Microsoft.AspNetCore.Mvc;

namespace RecambiosWOW.Api.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class TestController : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return Ok("RecambiosWOW API is working!");
    }
}