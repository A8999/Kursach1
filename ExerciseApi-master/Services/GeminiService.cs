using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
namespace ExerciseApi.Services
{
    public class GeminiService
    {
        private readonly string _apiKey;
        private readonly HttpClient _httpClient;
        public GeminiService(IConfiguration config)
        {
            _apiKey = config["Gemini:ApiKey"];
            _httpClient = new HttpClient();
        }

        public async Task<string> GetRecommendation(string planDescription)
        {
            var prompt = $"Ось мій тренувальний план: {planDescription}. Яких вправ не вистачає? Додай одну вправу з коротким описом і групою м'язів. Відповідь у форматі: Назва;Група;Опис.";
            var requestBody = new
            {
                contents = new[] {
                    new {
                        parts = new[] {
                            new { text = prompt }
                        }
                    }
                }
            };
            var content = new StringContent(System.Text.Json.JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");
            try
            {
                var url = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-1.5-flash-latest:generateContent?key={_apiKey}";
                var response = await _httpClient.PostAsync(url, content);
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    return $"[AI Error] Сталася помилка при зверненні до Gemini: {response.StatusCode}. {errorContent}";
                }
                var json = await response.Content.ReadAsStringAsync();
                var obj = JObject.Parse(json);
                var answer = obj["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();
                return answer;
            }
            catch (Exception ex)
            {
                return $"[AI Error] Внутрішня помилка: {ex.Message}";
            }
        }
    }
}
