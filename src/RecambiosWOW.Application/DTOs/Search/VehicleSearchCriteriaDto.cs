namespace RecambiosWOW.Application.DTOs.Search;

public class VehicleSearchCriteriaDto
{
    public string? SearchTerm { get; set; }
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public int Skip { get; set; } = 0;
    public int Take { get; set; } = 20;
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; }
}