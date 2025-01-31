using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RecambiosWOW.Application.Services;
using RecambiosWOW.Core.Exceptions;

namespace RecambiosWOW.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/vehicle-variants")]
[ApiVersion("1.0")]
public class VehicleVariantsController : ControllerBase
{
    private readonly IVehicleVariantService _variantService;
    private readonly ILogger<VehicleVariantsController> _logger;

    public VehicleVariantsController(
        IVehicleVariantService variantService,
        ILogger<VehicleVariantsController> logger)
    {
        _variantService = variantService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleVariantDto>> GetById(int id)
    {
        try
        {
            var variant = await _variantService.GetByIdAsync(id);
            return Ok(variant);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("model/{modelId}")]
    public async Task<ActionResult<IEnumerable<VehicleVariantDto>>> GetByModelId(int modelId)
    {
        var variants = await _variantService.GetByModelIdAsync(modelId);
        return Ok(variants);
    }

    [HttpGet("model/{modelId}/code/{code}")]
    public async Task<ActionResult<VehicleVariantDto>> GetByManufacturerCode(int modelId, string code)
    {
        try
        {
            var variant = await _variantService.GetByManufacturerCodeAsync(modelId, code);
            return Ok(variant);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<VehicleVariantDto>> Create(CreateVehicleVariantDto dto)
    {
        try
        {
            var created = await _variantService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById),
                new { id = created.Id },
                created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleVariantDto>> Update(int id, UpdateVehicleVariantDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updated = await _variantService.UpdateAsync(dto);
            return Ok(updated);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        try
        {
            var result = await _variantService.DeleteAsync(id);
            if (result)
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle variant with ID {Id}", id);
            throw;
        }
    }
}