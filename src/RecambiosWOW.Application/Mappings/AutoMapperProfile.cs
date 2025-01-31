using AutoMapper;
using RecambiosWOW.Application.DTOs;
using RecambiosWOW.Application.DTOs.Search;
using RecambiosWOW.Core.Domain.Entities;
using RecambiosWOW.Core.Domain.ValueObjects;
using RecambiosWOW.Core.Search;

namespace RecambiosWOW.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<Part, PartDto>().ReverseMap();
        CreateMap<Vehicle, VehicleDto>().ReverseMap();
        
        CreateMap<VehicleCompatibility, VehicleCompatibilityDto>()
            .ForMember(dest => dest.StartYear, 
                opt => opt.MapFrom(src => src.YearRange.StartYear))
            .ForMember(dest => dest.EndYear, 
                opt => opt.MapFrom(src => src.YearRange.EndYear))
            .ReverseMap()
            .ForPath(dest => dest.YearRange, 
                opt => opt.MapFrom(src => new YearRange(src.StartYear, src.EndYear)));

        // Vehicle Specification mappings
        CreateMap<VehicleSpecification, VehicleSpecificationDto>().ReverseMap();
        CreateMap<EngineDetails, EngineDetailsDto>().ReverseMap();
        CreateMap<Dimensions, DimensionsDto>().ReverseMap();
        CreateMap<VehicleDetails, VehicleDetailsDto>().ReverseMap();
        
        // Search Criteria mappings
        CreateMap<PartSearchCriteriaDto, PartSearchCriteria>()
            .ConstructUsing(src => new PartSearchCriteria(
                src.SearchTerm,
                src.Condition,
                src.Manufacturer,
                src.MaxPrice,
                src.MinPrice,
                src.Make,
                src.Model,
                src.Year,
                src.Skip,
                src.Take,
                src.SortBy,
                src.SortDescending
            ));   
        
        CreateMap<VehicleSearchCriteriaDto, VehicleSearchCriteria>()
            .ConstructUsing(src => new VehicleSearchCriteria(
                src.SearchTerm,
                src.Make,
                src.Model,
                src.Year,
                src.EngineCode,
                src.FuelType,
                src.TransmissionType,
                src.BodyType,
                src.LicensePlate,
                src.VIN,
                src.Skip,
                src.Take,
                src.SortBy,
                src.SortDescending
            ));       
    }
}