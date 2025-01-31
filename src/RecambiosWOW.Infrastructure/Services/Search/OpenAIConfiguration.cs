namespace RecambiosWOW.Infrastructure.Services.Search;

public class OpenAIConfiguration
{
    public string ApiKey { get; set; }
    public string Model { get; set; } = "gpt-4";
    public int MaxTokens { get; set; } = 150;
}