using System.Text.Json;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.ValueObjects;
using VehicleCompatibility = RecambiosWOW.Core.Domain.ValueObjects.VehicleCompatibility;
using VehicleSpecification = RecambiosWOW.Core.Domain.Entities.VehicleSpecification;

namespace RecambiosWOW.Infrastructure.Data.Models;

public static class DomainToModelMapper
{
    public static PartModel ToModel(this Part entity)
    {
        return new PartModel
        {
            Id = entity.Id,
            Manufacturer = entity.Identifier.Manufacturer,
            PartNumber = entity.Identifier.PartNumber,
            SerialNumber = entity.Identifier.SerialNumber,
            Name = entity.Name,
            Description = entity.Description,
            Condition = entity.Condition,
            PriceAmount = entity.Price.Amount,
            PriceCurrency = entity.Price.Currency,
            Source = entity.Source,
            CompatibilityJson = JsonSerializer.Serialize(entity.Compatibility),
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt
        };
    }

    public static Part ToDomain(this PartModel model)
    {
        var part = new Part(
            new PartIdentifier(model.Manufacturer, model.PartNumber, model.SerialNumber),
            model.Name,
            model.Description,
            model.Condition,
            new Price(model.PriceAmount, model.PriceCurrency),
            model.Source);

        // Use reflection or a private method to set the Id
        typeof(Part).GetProperty(nameof(Part.Id))
            ?.SetValue(part, model.Id);

        var compatibility = JsonSerializer.Deserialize<List<VehicleCompatibility>>(
            model.CompatibilityJson ?? "[]");

        foreach (var compat in compatibility)
        {
            part.AddCompatibility(compat);
        }

        return part;
    }

    public static VehicleModel ToModel(this Vehicle entity)
    {
        return new VehicleModel
        {
            Id = entity.Id,
            VIN = entity.VIN,
            LicensePlate = entity.LicensePlate,
            Make = entity.Make,
            Model = entity.Model,
            Year = entity.Year,
            SpecificationsJson = JsonSerializer.Serialize(entity.Specifications)
        };
    }

    public static Vehicle ToDomain(this VehicleModel model)
    {
        VehicleSpecification? specifications;
        if (model.SpecificationsJson != null)
        {
            specifications = JsonSerializer.Deserialize<VehicleSpecification>(
                model.SpecificationsJson);
        }
        else
        {
            specifications = new VehicleSpecification();
        }

        var vehicle = new Vehicle(
            new VehicleIdentifier(model.Make, model.Model, model.Year),
            model.VIN,
            model.LicensePlate,
            specifications);

        // Use reflection or a private method to set the Id
        typeof(Vehicle).GetProperty(nameof(Vehicle.Id))
            ?.SetValue(vehicle, model.Id);
        
        return vehicle;
    }
}
