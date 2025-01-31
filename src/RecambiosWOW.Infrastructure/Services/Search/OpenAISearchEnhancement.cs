using System.Net.Http.Headers;
using System.Net.Http.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using RecambiosWOW.Core.Interfaces.Services;
using RecambiosWOW.Core.Search.SearchEnhancement;

namespace RecambiosWOW.Infrastructure.Services.Search;

public class OpenAISearchEnhancement : ISearchEnhancementService{
    private readonly HttpClient _httpClient;
    private readonly ICacheService _cacheService;
    private readonly ILogger<OpenAISearchEnhancement> _logger;
    private readonly OpenAIConfiguration _configuration;

    public OpenAISearchEnhancement(
        HttpClient httpClient,
        ICacheService cacheService,
        IOptions<OpenAIConfiguration> configuration,
        ILogger<OpenAISearchEnhancement> logger)
    {
        _httpClient = httpClient;
        _cacheService = cacheService;
        _configuration = configuration.Value;
        _logger = logger;

        _httpClient.BaseAddress = new Uri("https://api.openai.com/v1/");
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _configuration.ApiKey);
    }

    public async Task<IEnumerable<string>> GenerateSearchTermsAsync(string query)
    {
        var cacheKey = $"search:terms:{query.GetHashCode()}";
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            try
            {
                var prompt = $@"Generate 3-5 alternative search terms or phrases for finding auto parts similar to: '{query}'.
                Focus on technical terms, common variations, and related part numbers if applicable.
                Return only the terms, one per line.";

                var response = await GetCompletionAsync(prompt);
                return response
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating search terms for query: {Query}", query);
                return new[] { query };
            }
        }, TimeSpan.FromHours(24));
    }

    public async Task<IEnumerable<string>> ExtractKeywordsAsync(string content)
    {
        var cacheKey = $"keywords:{content.GetHashCode()}";
        return await _cacheService.GetOrCreateAsync(cacheKey, async () =>
        {
            try
            {
                var prompt = $@"Extract key technical terms and identifiers from this auto part description. 
                Include part numbers, specifications, and technical terminology.
                Return only the keywords, one per line: 

                {content}";

                var response = await GetCompletionAsync(prompt);
                return response
                    .Split('\n', StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrWhiteSpace(s));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error extracting keywords from content");
                return Array.Empty<string>();
            }
        }, TimeSpan.FromDays(7));
    }

    public async Task<double> CalculateRelevanceScoreAsync(string query, string content)
    {
        try
        {
            var prompt = $@"On a scale of 0 to 1, how relevant is this auto part to the search query?
            Consider technical matches, functional similarity, and compatibility.
            Return only a number between 0 and 1.

            Query: {query}
            Content: {content}";

            var response = await GetCompletionAsync(prompt);
            if (double.TryParse(response, out double score))
            {
                return score;
            }
            return 1.0;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error calculating relevance score");
            return 1.0;
        }
    }

    private async Task<string> GetCompletionAsync(string prompt)
    {
        var request = new
        {
            model = _configuration.Model,
            messages = new[]
            {
                new { role = "system", content = "You are an auto parts expert helping with search optimization." },
                new { role = "user", content = prompt }
            },
            max_tokens = _configuration.MaxTokens,
            temperature = 0.3
        };

        var response = await _httpClient.PostAsJsonAsync("chat/completions", request);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<OpenAIResponse>();
        return result?.Choices?.FirstOrDefault()?.Message?.Content?.Trim() ?? string.Empty;
    }

    private class OpenAIResponse
    {
        public Choice[] Choices { get; set; }
        
        public class Choice
        {
            public Message Message { get; set; }
        }
        
        public class Message
        {
            public string Content { get; set; }
        }
    }
}