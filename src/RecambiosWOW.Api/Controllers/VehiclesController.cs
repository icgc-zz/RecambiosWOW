using Microsoft.AspNetCore.Mvc;
using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.Interfaces;

namespace RecambiosWOW.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiVersion("1.0")]
public class PartsController : ControllerBase
{
    private readonly IPartService _partService;

    public PartsController(IPartService partService)
    {
        _partService = partService;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<PartDto>> GetById(string id)
    {
        var part = await _partService.GetPartByIdAsync(id);
        return Ok(part);
    }
}