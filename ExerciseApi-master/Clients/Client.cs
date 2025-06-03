using APIprot.body;
using Newtonsoft.Json;
using System.Reflection.Metadata;

namespace APIprot.Clients
{
    public class Client
    {
        private readonly HttpClient _client;

        public Client()
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(Constant.Address);
            _client.DefaultRequestHeaders.Add("X-RapidAPI-Key", Constant.ApiKey);
            _client.DefaultRequestHeaders.Add("X-RapidAPI-Host", Constant.ApiHost);
        }

        public async Task<List<Body>> GetExercises(string bodyPart)
        {
            var response = await _client.GetAsync($"/exercises/bodyPart/{bodyPart}");
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            var results = JsonConvert.DeserializeObject<List<Body>>(content);

            return results;
        }

    }
}
