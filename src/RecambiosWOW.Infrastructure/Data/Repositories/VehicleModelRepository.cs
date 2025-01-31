using RecambiosWOW.Core.Domain.Criteria;
using RecambiosWOW.Core.Domain.ValueObjects;
using RecambiosWOW.Infrastructure.Data.Models;

public class VehicleModelRepository : IVehicleModelRepository
{
   private readonly IDataStore _dataStore;

   public VehicleModelRepository(IDataStore dataStore)
   {
       _dataStore = dataStore;
   }

   public Task<VehicleModel> GetByIdAsync(int id)
       => _dataStore.GetByIdAsync<VehicleModel>(id);

   public Task<VehicleModel> GetByDetailsAsync(string make, string model, int year)
       => _dataStore.SingleAsync<VehicleModel>(x => 
           x.Make == make && 
           x.Model == model && 
           x.Year == year);

   public Task<IEnumerable<VehicleModel>> SearchAsync(VehicleModelSearchCriteria criteria)
   {
       var query = BuildQuery(criteria);
       return _dataStore.QueryAsync(
           query, 
           criteria.Skip, 
           criteria.Take, 
           GetSortExpression(criteria));
   }

   public Task<int> GetTotalCountAsync(VehicleModelSearchCriteria criteria)
       => _dataStore.CountAsync(BuildQuery(criteria));

   public Task<VehicleModel> AddAsync(VehicleModel model)
       => _dataStore.AddAsync(model);

   public Task<VehicleModel> UpdateAsync(VehicleModel model) 
       => _dataStore.UpdateAsync(model);

   public Task<bool> DeleteAsync(int id)
       => _dataStore.DeleteAsync<VehicleModel>(id);

   private IQuerySpecification<VehicleModel> BuildQuery(VehicleModelSearchCriteria criteria)
   {
       var spec = new QuerySpecification<VehicleModel>();

       if (!string.IsNullOrWhiteSpace(criteria.SearchTerm))
           spec.AddCriteria(x => x.Make.Contains(criteria.SearchTerm) || 
                               x.Model.Contains(criteria.SearchTerm));

       if (!string.IsNullOrWhiteSpace(criteria.Make))
           spec.AddCriteria(x => x.Make == criteria.Make);

       if (!string.IsNullOrWhiteSpace(criteria.Model))
           spec.AddCriteria(x => x.Model == criteria.Model);

       if (criteria.Year.HasValue)
           spec.AddCriteria(x => x.Year == criteria.Year);

       if (criteria.YearFrom.HasValue)
           spec.AddCriteria(x => x.Year >= criteria.YearFrom);

       if (criteria.YearTo.HasValue)
           spec.AddCriteria(x => x.Year <= criteria.YearTo);

       if (criteria.IncludeVariants)
           spec.AddInclude(x => x.Variants);

       return spec;
   }

   private string GetSortExpression(VehicleModelSearchCriteria criteria)
   {
       return criteria.SortBy?.ToLower() switch
       {
           "make" => criteria.SortDescending ? "Make DESC" : "Make",
           "model" => criteria.SortDescending ? "Model DESC" : "Model",
           "year" => criteria.SortDescending ? "Year DESC" : "Year",
           _ => "Id"
       };
   }
}