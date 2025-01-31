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

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PartDto>> GetById(int id)
    {
        var part = await _partService.GetByIdAsync(id);
        return Ok(part);
    }
    public async Task<ActionResult<PartDto>> GetBySerialNumber(string serial)
    {
        var part = await _partService.GetBySerialNumberAsync(serial);
        return Ok(part);
    }
    
}