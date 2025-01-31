using SQLite;

namespace RecambiosWOW.Infrastructure.Services.Search;

[Table("PartsSearch")]
public class SearchableVehicleModel
{
    [PrimaryKey]
    public int Id { get; set; }
    public string Content { get; set; }  // Combined searchable content
    public string Keywords { get; set; }  // AI-generated keywords
    public DateTime LastUpdated { get; set; }
}