using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using VshghCoachBot.Models;

public class Client
{
    private readonly HttpClient _client;

    public Client()
    {
        _client = new HttpClient();
        _client.BaseAddress = new Uri("http://localhost:5100");
    }
    public async Task SetUserParameters(long userId, double height, double weight)
    {
        var dto = new { height, weight };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _client.PostAsync($"/api/userparameters/{userId}", content);
        resp.EnsureSuccessStatusCode();
    }

    public async Task<(double height, double weight)?> GetUserParameters(long userId)
    {
        var resp = await _client.GetAsync($"/api/userparameters/{userId}");
        if (!resp.IsSuccessStatusCode) return null;
        var json = await resp.Content.ReadAsStringAsync();
        var obj = JsonConvert.DeserializeObject<UserParamsDto>(json);
        return (obj.Height, obj.Weight);
    }

    public async Task DeleteUserParameters(long userId)
    {
        var resp = await _client.DeleteAsync($"/api/userparameters/{userId}");
        resp.EnsureSuccessStatusCode();
    }

    private class UserParamsDto
    {
        public double Height { get; set; }
        public double Weight { get; set; }
    }
    public async Task<List<ExerciseDto>> GetExercises(string bodyPart)
    {
        var response = await _client.GetAsync($"/api/Exercise/all?bodyPart={bodyPart}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<ExerciseDto>>(content);
    }

    public async Task<string> GetExerciseNameById(string id)
    {
        var response = await _client.GetAsync($"/api/Exercise/getbyid/{id}");
        if (!response.IsSuccessStatusCode) return null;
        var content = await response.Content.ReadAsStringAsync();
        var exercise = JsonConvert.DeserializeObject<ExerciseDto>(content);
        return exercise?.Name;
    }
    public async Task AddFavoriteExercise(long telegramId, string exerciseId, string name)
    {
        var payload = new
        {
            TelegramId = telegramId,
            ExerciseId = exerciseId,
            ExerciseName = name
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/Favorite/add", content);
        response.EnsureSuccessStatusCode();
    }
    public class RemoveFavoriteResult
    {
        public bool removed { get; set; }
    }

    public async Task<bool> RemoveFavoriteExercise(long telegramId, string exerciseId)
    {
        var response = await _client.DeleteAsync($"/api/Favorite/remove/{telegramId}/{exerciseId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RemoveFavoriteResult>(content);
        return result != null && result.removed;
    }

    public async Task<List<FavoriteExerciseDto>> GetFavoriteExercises(long telegramId)
    {
        var response = await _client.GetAsync($"/api/Favorite/user/{telegramId}");
        response.EnsureSuccessStatusCode();

        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<FavoriteExerciseDto>>(content);
    }
    //plans
    public async Task CreateTrainingPlan(long telegramId, string planName)
    {
        var payload = new
        {
            TelegramId = telegramId,
            PlanName = planName
        };

        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _client.PostAsync("/api/TrainingPlan/create", content);
        response.EnsureSuccessStatusCode();
    }
    public class PlanExerciseDto
    {
        public string ExerciseId { get; set; }
        public string ExerciseName { get; set; }
    }
    public class TrainingPlanDto
    {
        public string PlanName { get; set; }
        public List<PlanExerciseDto> Exercises { get; set; }
    }
    public async Task AddExerciseToPlan(long telegramId, string planName, string exerciseId, string exerciseName)
    {
        var payload = new
        {
            TelegramId = telegramId,
            PlanName = planName,
            ExerciseId = exerciseId,
            ExerciseName = exerciseName
        };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/TrainingPlan/addExercise", content);
        response.EnsureSuccessStatusCode();
    }
    public async Task<bool> RemoveExerciseFromPlan(long telegramId, string planName, string exerciseId)
    {
        var payload = new
        {
            TelegramId = telegramId,
            PlanName = planName,
            ExerciseId = exerciseId
        };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/TrainingPlan/removeExercise", content);
        response.EnsureSuccessStatusCode();
        var respContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<RemoveFavoriteResult>(respContent);
        return result != null && result.removed;
    }
    public async Task<List<TrainingPlanDto>> GetUserPlans(long telegramId)
    {
        var response = await _client.GetAsync($"/api/TrainingPlan/user/{telegramId}");
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<List<TrainingPlanDto>>(content);
    }
    public async Task EditExerciseInPlan(long telegramId, string planName, string oldExerciseId, string newExerciseId, string newExerciseName)
    {
        var payload = new
        {
            TelegramId = telegramId,
            PlanName = planName,
            OldExerciseId = oldExerciseId,
            NewExerciseId = newExerciseId,
            NewExerciseName = newExerciseName
        };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/TrainingPlan/editExercise", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task MarkPlanAsDone(long telegramId, string planName)
    {
        var payload = new { TelegramId = telegramId, PlanName = planName };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/TrainingPlan/doneplan", content);
        response.EnsureSuccessStatusCode();
    }

    //AI
    public class AiRecommendResult
    {
        public string added { get; set; }
        public string bodyPart { get; set; }
        public string aiResponse { get; set; }
    }
    public async Task<AiRecommendResult> AiRecommend(long telegramId, string planName)
    {
        try
        {
            var payload = new { TelegramId = telegramId, PlanName = planName };
            var json = JsonConvert.SerializeObject(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/TrainingPlan/ai-recommend", content);
            response.EnsureSuccessStatusCode();
            var respContent = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AiRecommendResult>(respContent);
            if (result == null || (string.IsNullOrEmpty(result.added) && string.IsNullOrEmpty(result.bodyPart)))
            {
                var obj = JsonConvert.DeserializeObject<dynamic>(respContent);
                if (obj != null && obj.message != null)
                {
                    return new AiRecommendResult { aiResponse = (string)obj.message };
                }
            }
            return result;
        }
        catch (HttpRequestException ex)
        {
            return new AiRecommendResult { aiResponse = $"[AI Error] Проблема з підключенням до серверу: {ex.Message}" };
        }
        catch (Exception ex)
        {
            return new AiRecommendResult { aiResponse = $"[AI Error] Невідома помилка: {ex.Message}" };
        }
    }

    //SBD
    public async Task SetSBD(long userId, double squat, double bench, double deadlift)
    {
        var dto = new { userId, squat, bench, deadlift };
        var json = JsonConvert.SerializeObject(dto);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var resp = await _client.PostAsync($"/api/sbd", content);
        resp.EnsureSuccessStatusCode();
    }

    public async Task DeleteSBD(long userId)
    {
        var resp = await _client.DeleteAsync($"/api/sbd/{userId}");
        resp.EnsureSuccessStatusCode();
    }

    public async Task<(double squat, double bench, double deadlift)?> GetSBD(long userId)
    {
        var resp = await _client.GetAsync($"/api/sbd/{userId}");
        if (!resp.IsSuccessStatusCode) return null;
        var json = await resp.Content.ReadAsStringAsync();
        var obj = Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(json);
        return ((double)obj.squat, (double)obj.bench, (double)obj.deadlift);
    }

    public async Task<bool> DeleteTrainingPlan(long telegramId, string planName)
    {
        var payload = new
        {
            TelegramId = telegramId,
            PlanName = planName
        };
        var json = JsonConvert.SerializeObject(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await _client.PostAsync("/api/TrainingPlan/delete", content);
        if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            return false;
        var respContent = await response.Content.ReadAsStringAsync();
        var result = JsonConvert.DeserializeObject<DeletePlanResult>(respContent);
        return result != null && result.deleted;
    }
    public class DeletePlanResult { public bool deleted { get; set; } }
}
















