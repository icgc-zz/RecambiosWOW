using RecambiosWOW.Core.Domain.Enums;

namespace RecambiosWOW.Application.DTOs.Search;

public class PartSearchCriteriaDto
{
    public string SearchTerm { get; set; }
    public PartCondition? Condition { get; set; }
    public string Manufacturer { get; set; }
    public decimal? MaxPrice { get; set; }
    public decimal? MinPrice { get; set; }
    public string Make { get; set; }
    public string Model { get; set; }
    public int? Year { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}