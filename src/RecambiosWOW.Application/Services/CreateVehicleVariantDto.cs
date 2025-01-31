﻿using RecambiosWOW.Application.DTOs;

namespace RecambiosWOW.Application.Services;

public record CreateVehicleVariantDto
{
    public int ModelId { get; init; }
    public string ManufacturerCode { get; init; } = string.Empty;
    public EngineDetailsDto Engine { get; init; } = null!;
    public DimensionsDto Dimensions { get; init; } = null!;
    public VehicleDetailsDto Details { get; init; } = null!;
}