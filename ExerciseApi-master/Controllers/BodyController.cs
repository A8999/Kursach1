using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Mvc;
using VshghCoachBot.Models;
using System.Collections.Generic;
using System.Linq;

namespace APIprot.Controllers
{
    [ApiController]
    [Route("api/Exercise")]
    public class BodyController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private const string ApiUrl = "https://exercisedb.p.rapidapi.com/exercises/bodyPart/";
        private const string ApiKey = "affebf4d92msh84beee544823187p1ece4ejsnbfa3c55fdb71";
        private const string ApiHost = "exercisedb.p.rapidapi.com";

        public BodyController()
        {
            _httpClient = new HttpClient();
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Key", ApiKey);
            _httpClient.DefaultRequestHeaders.Add("X-RapidAPI-Host", ApiHost);
        }

        // exer bodyPart
        [HttpGet("all")]
        public async Task<List<ExerciseDto>> GetAll([FromQuery] string bodyPart)
        {
            var url = ApiUrl + bodyPart;
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var json = await response.Content.ReadAsStringAsync();
            var exercises = JsonConvert.DeserializeObject<List<ExerciseDto>>(json);
            return exercises;
        }

        // name id
        [HttpGet("getname/{id}")]
        public async Task<ActionResult<string>> GetName(string id)
        {
            var url = "https://exercisedb.p.rapidapi.com/exercises/exercise/" + id;
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return NotFound();
            var json = await response.Content.ReadAsStringAsync();
            var exercise = JsonConvert.DeserializeObject<ExerciseDto>(json);
            return exercise?.Name ?? "Невідома вправа";
        }

        // exer id
        [HttpGet("getbyid/{id}")]
        public async Task<ActionResult<ExerciseDto>> GetById(string id)
        {
            var url = "https://exercisedb.p.rapidapi.com/exercises/exercise/" + id;
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode) return NotFound();
            var json = await response.Content.ReadAsStringAsync();
            var exercise = JsonConvert.DeserializeObject<ExerciseDto>(json);
            if (exercise == null) return NotFound();
            return exercise;
        }
    }
}
