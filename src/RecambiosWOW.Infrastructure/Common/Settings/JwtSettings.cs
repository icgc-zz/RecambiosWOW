// src/RecambiosWOW.Infrastructure/Common/Settings/JwtSettings.cs
using System.ComponentModel.DataAnnotations;

namespace RecambiosWOW.Infrastructure.Common.Settings;

public class JwtSettings
{
    [Required]
    public string SecretKey { get; set; } = string.Empty;

    [Required]
    public string Issuer { get; set; } = string.Empty;

    [Required]
    public string Audience { get; set; } = string.Empty;

    [Range(1, 24 * 30)] // Maximum 30 days
    public int ExpirationHours { get; set; } = 1;
}