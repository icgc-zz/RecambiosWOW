namespace RecambiosWOW.Core.Search.SearchEnhancement;

public interface ISearchEnhancementService
{
    Task<IEnumerable<string>> GenerateSearchTermsAsync(string query);
    Task<double> CalculateRelevanceScoreAsync(string query, string content);
    Task<IEnumerable<string>> ExtractKeywordsAsync(string content);
}