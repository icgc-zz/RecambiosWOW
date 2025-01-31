using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;
using RecambiosWOW.Application.Interfaces;
using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Exceptions;

namespace RecambiosWOW.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/vehicle-models")]
[ApiVersion("1.0")]
public class VehicleModelsController : ControllerBase
{
    private readonly IVehicleModelService _vehicleModelService;
    private readonly ILogger<VehicleModelsController> _logger;

    public VehicleModelsController(
        IVehicleModelService vehicleModelService,
        ILogger<VehicleModelsController> logger)
    {
        _vehicleModelService = vehicleModelService;
        _logger = logger;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<VehicleModelDto>> GetById(int id)
    {
        try
        {
            var model = await _vehicleModelService.GetByIdAsync(id);
            return Ok(model);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpGet("search")]
    public async Task<ActionResult<IEnumerable<VehicleModelDto>>> Search([FromQuery] VehicleModelSearchCriteria criteria)
    {
        var models = await _vehicleModelService.SearchAsync(criteria);
        return Ok(models);
    }

    [HttpGet("details")]
    public async Task<ActionResult<VehicleModelDto>> GetByDetails(
        [FromQuery] string make,
        [FromQuery] string model,
        [FromQuery] int year)
    {
        try
        {
            var vehicleModel = await _vehicleModelService.GetByDetailsAsync(make, model, year);
            return Ok(vehicleModel);
        }
        catch (NotFoundException ex)
        {
            return NotFound(ex.Message);
        }
    }

    [HttpPost]
    public async Task<ActionResult<VehicleModelDto>> Create(CreateVehicleModelDto dto)
    {
        try
        {
            var created = await _vehicleModelService.CreateAsync(dto);
            return CreatedAtAction(
                nameof(GetById), 
                new { id = created.Id }, 
                created);
        }
        catch (ValidationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<VehicleModelDto>> Update(int id, UpdateVehicleModelDto dto)
    {
        if (id != dto.Id)
            return BadRequest("ID mismatch");

        try
        {
            var updated = await _vehicleModelService.UpdateAsync(dto);
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
            var result = await _vehicleModelService.DeleteAsync(id);
            if (result)
                return NoContent();
            return NotFound();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting vehicle model with ID {Id}", id);
            throw;
        }
    }
}