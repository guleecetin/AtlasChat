using System.Text;
using System.Text.Json;

namespace AtlasChat.Services;

public interface IAIService
{
    Task<string> GetResponseAsync(string userMessage);
}

public class OllamaAIService : IAIService
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<OllamaAIService> _logger;

    public OllamaAIService(HttpClient httpClient, ILogger<OllamaAIService> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task<string> GetResponseAsync(string userMessage)
    {
        try
        {
            var requestBody = new
            {
                model = "llama3",
                prompt = userMessage,
                stream = false
            };

            var json = JsonSerializer.Serialize(requestBody);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(
                "http://localhost:11434/api/generate", content);

            response.EnsureSuccessStatusCode();

            var responseJson = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(responseJson);

            var text = doc.RootElement
                .GetProperty("response")
                .GetString();

            return text ?? "Cevap alınamadı.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Ollama API hatası.");
            return GetFallbackResponse(userMessage);
        }
    }

    private static string GetFallbackResponse(string message)
    {
        var lower = message.ToLowerInvariant();

        if (lower.Contains("merhaba") || lower.Contains("selam"))
            return "Merhaba! Size nasıl yardımcı olabilirim?";

        if (lower.Contains("teşekkür"))
            return "Rica ederim! Başka bir konuda yardımcı olabilir miyim?";

        if (lower.Contains("görüşürüz") || lower.Contains("hoşça kal"))
            return "Görüşmek üzere! İyi günler dileriz.";

        return "Anlıyorum, bu konuda size yardımcı olmaya çalışayım.";
    }
}