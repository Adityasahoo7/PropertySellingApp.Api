using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Net.Http.Headers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using PropertySellingApp.Models.DTOs;
using PropertySellingApp.Services.Interfaces;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;
using System;

public class GroqAiService : IAiService
{
    private readonly HttpClient _http;
    private readonly string _modelName;
    private readonly ILogger<GroqAiService> _logger;

    public GroqAiService(HttpClient http, IConfiguration cfg, ILogger<GroqAiService> logger)
    {
        _http = http;
        _logger = logger;

        _http.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", cfg["Groq:ApiKey"]);
        _modelName = cfg["Groq:ModelName"] ?? "gpt-3.5-turbo";
    }

    public async Task<AiSearchResult?> ParseSearchQueryAsync(string naturalQuery)
    {
        if (string.IsNullOrWhiteSpace(naturalQuery)) return null;

        _logger.LogInformation("Parsing AI search query: {Query}", naturalQuery);


        var prompt = string.Format(@"
You are a property search parser for a real estate database with these columns:
Title, Description, Type, Price, Location, Bedrooms, Bathrooms, AreaSqFt.

User query: ""{0}""

Your task:
Return only valid JSON. No explanation, no code block, no markdown.

JSON format:
{{
  ""Keywords"": ""string"",
  ""Location"": ""string"",
  ""Price"": null,
  ""Bedrooms"": null,
  ""Bathrooms"": null,
  ""Type"": ""string"",
  ""AreaSqFt"": null
}}
", naturalQuery);






        var payload = new
    {
        model = _modelName,
        messages = new[]
        {
            new { role = "system", content = "You are a helpful assistant that returns only valid JSON." },
            new { role = "user", content = prompt }
        }
    };

        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        try
        {
            var resp = await _http.PostAsync("chat/completions", content);
            _logger.LogInformation("Groq API called, status code: {StatusCode}", resp.StatusCode);

            resp.EnsureSuccessStatusCode();
            var txt = await resp.Content.ReadAsStringAsync();

            _logger.LogInformation("Groq API raw response: {Response}", txt);

            using var doc = JsonDocument.Parse(txt);
            var contentText = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            if (string.IsNullOrWhiteSpace(contentText))
            {
                _logger.LogWarning("AI response content is empty.");
                return null;
            }

            // Clean and extract JSON
            var cleaned = Regex.Replace(contentText, @"[`]+", "").Trim();
            var start = cleaned.IndexOf('{');
            var end = cleaned.LastIndexOf('}');
            if (start < 0 || end < 0 || end <= start)
            {
                _logger.LogWarning("No valid JSON found in AI content: {Content}", cleaned);
                return null;
            }

            var jsonOnly = cleaned.Substring(start, end - start + 1);

            _logger.LogInformation("🧹 Cleaned JSON for deserialization: {Json}", jsonOnly);

            try
            {
                var result = JsonSerializer.Deserialize<AiSearchResult>(
                    jsonOnly,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                );

                if (result == null)
                    _logger.LogWarning("Deserialized result is null from AI content: {Json}", jsonOnly);

                return result;
            }
            catch (JsonException ex)
            {
                _logger.LogError(ex, "❌ Failed to deserialize Groq AI JSON. Raw content: {Json}", jsonOnly);
                return null;
            }
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "❌ HTTP error calling Groq AI API");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "❌ Unexpected error calling Groq AI API");
            throw;
        }
    }


    private decimal? ParseIndianNumberToDecimal(string input)
    {
        if (string.IsNullOrWhiteSpace(input)) return null;
        input = input.ToLower().Trim().Replace(",", "");

        var match = Regex.Match(input, @"([\d\.]+)\s*(crore|cr|lakh|lac|k|thousand|m|million)?");
        if (!match.Success) return null;

        decimal number = Convert.ToDecimal(match.Groups[1].Value);
        var unit = match.Groups[2].Value;

        return unit switch
        {
            "crore" or "cr" => number * 10000000m,
            "lakh" or "lac" => number * 100000m,
            "k" or "thousand" => number * 1000m,
            "m" or "million" => number * 1000000m,
            _ => number
        };
    }
}
