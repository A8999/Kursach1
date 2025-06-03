using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ExerciseApi.Models;
using ExerciseApi.Data;

namespace APIprot.Controllers
{
    [ApiController]
    [Route("api/TrainingPlan")]
    public class TrainingPlanController : ControllerBase
    {
        private readonly AppDbContext _db;
        public TrainingPlanController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("create")]
        public IActionResult Create([FromBody] PlanDto dto)
        {
            if (!_db.PlanExercises.Any(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName))
            {
            }
            return Ok();
        }

        [HttpPost("addExercise")]
        public IActionResult AddExercise([FromBody] AddExerciseToPlanDto dto)
        {
            if (!_db.PlanExercises.Any(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName && x.ExerciseId == dto.ExerciseId))
            {
                _db.PlanExercises.Add(new PlanExercise
                {
                    UserId = dto.TelegramId,
                    PlanName = dto.PlanName,
                    ExerciseId = dto.ExerciseId,
                    ExerciseName = dto.ExerciseName
                });
                _db.SaveChanges();
            }
            return Ok();
        }
        [HttpPost("removeExercise")]
        public IActionResult RemoveExercise([FromBody] RemoveExerciseFromPlanDto dto)
        {
            var exId = dto.ExerciseId.Trim();
            var items = _db.PlanExercises.Where(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName && x.ExerciseId.ToLower() == exId.ToLower()).ToList();
            var allIds = _db.PlanExercises.Where(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName).Select(x => x.ExerciseId).ToList();
            Console.WriteLine($"[RemoveExercise] Запит на видалення: '{exId}', у плані: [{string.Join(", ", allIds)}]");
            if (items.Count > 0)
            {
                _db.PlanExercises.RemoveRange(items);
                _db.SaveChanges();
                return Ok(new { removed = true });
            }
            return Ok(new { removed = false });
        }
        [HttpPost("editExercise")]
        public IActionResult EditExercise([FromBody] EditExerciseInPlanDto dto)
        {
            var item = _db.PlanExercises.FirstOrDefault(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName && x.ExerciseId == dto.OldExerciseId);
            if (item != null)
            {
                item.ExerciseId = dto.NewExerciseId;
                item.ExerciseName = dto.NewExerciseName;
                _db.PlanExercises.Update(item);
                _db.SaveChanges();
            }
            return Ok();
        }
        [HttpGet("user/{telegramId}")]
        public ActionResult<List<TrainingPlanDto>> GetPlans(long telegramId)
        {
            var plans = _db.PlanExercises
                .Where(x => x.UserId == telegramId)
                .GroupBy(x => x.PlanName)
                .Select(g => new TrainingPlanDto
                {
                    PlanName = g.Key,
                    Exercises = g.Select(e => new PlanExerciseDto { ExerciseId = e.ExerciseId, ExerciseName = e.ExerciseName }).ToList()
                })
                .ToList();
            return plans;
        }
        [HttpPost("ai-recommend")]
        public async Task<IActionResult> AiRecommend([FromBody] AiRecommendDto dto, [FromServices] ExerciseApi.Services.GeminiService geminiService)
        {
            // 1. Отримати вправи з плану
            var planExercises = _db.PlanExercises.Where(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName).ToList();
            if (planExercises.Count == 0)
                return BadRequest("План порожній або не знайдено");
            // 2. Сформувати опис плану
            var planDescription = string.Join(", ", planExercises.Select(e => $"{e.ExerciseName} ({e.ExerciseId})"));
            // 3. Викликати Gemini
            var aiResponse = await geminiService.GetRecommendation(planDescription);
            if (aiResponse != null && aiResponse.StartsWith("[AI Error]"))
            {
                // Якщо сталася помилка AI, повертаємо її як message
                return Ok(new { message = aiResponse });
            }
            // 4. Парсити відповідь (очікуємо формат: Назва;Група;Опис)
            var parts = aiResponse?.Split(';');
            if (parts == null || parts.Length < 2)
                return Ok(new { message = "AI не зміг запропонувати вправу.", aiResponse });
            var exerciseName = parts[0].Trim();
            var bodyPart = parts[1].Trim();
            // 5. Додати вправу у план (id буде "AI" + 4 випадкові цифри)
            var random = new Random();
            var shortId = $"AI-{random.Next(1000, 10000)}";
            var newExercise = new PlanExercise
            {
                UserId = dto.TelegramId,
                PlanName = dto.PlanName,
                ExerciseId = shortId,
                ExerciseName = exerciseName
            };
            _db.PlanExercises.Add(newExercise);
            _db.SaveChanges();
            return Ok(new { added = exerciseName, bodyPart, aiResponse });
        }

        [HttpPost("delete")]
        public IActionResult DeletePlan([FromBody] PlanDto dto)
        {
            // Знаходимо всі вправи у плані
            var items = _db.PlanExercises.Where(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName).ToList();
            // Перевіряємо, чи існує хоча б один запис з цим планом (навіть якщо немає вправ)
            bool planExists = _db.PlanExercises.Any(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName) ||
                              _db.PlanExercises.Local.Any(x => x.UserId == dto.TelegramId && x.PlanName == dto.PlanName);
            if (!planExists)
                return NotFound(new { deleted = false });
            // Видаляємо всі вправи з плану (якщо є)
            if (items.Count > 0)
            {
                _db.PlanExercises.RemoveRange(items);
                _db.SaveChanges();
            }
            // Додаємо "заглушку" для порожнього плану, якщо потрібно (або видаляємо, якщо є окрема таблиця для планів)
            // Якщо план існує лише як "порожній" (без вправ), його теж вважаємо видаленим
            return Ok(new { deleted = true });
        }

        public class TrainingPlanDto
        {
            public string PlanName { get; set; }
            public List<PlanExerciseDto> Exercises { get; set; } = new();
        }
        public class PlanExerciseDto
        {
            public string ExerciseId { get; set; }
            public string ExerciseName { get; set; }
        }
        public class TrainingHistoryDto
        {
            public string PlanName { get; set; }
            public List<PlanExerciseDto> Exercises { get; set; }
            public DateTime Date { get; set; } = DateTime.UtcNow;
        }
        public class DonePlanDto
        {
            public long TelegramId { get; set; }
            public string PlanName { get; set; }
        }
        public class PlanDto
        {
            public long TelegramId { get; set; }
            public string PlanName { get; set; }
        }
        public class AddExerciseToPlanDto
        {
            public long TelegramId { get; set; }
            public string PlanName { get; set; }
            public string ExerciseId { get; set; }
            public string ExerciseName { get; set; }
        }
        public class RemoveExerciseFromPlanDto
        {
            public long TelegramId { get; set; }
            public string PlanName { get; set; }
            public string ExerciseId { get; set; }
        }
        public class EditExerciseInPlanDto
        {
            public long TelegramId { get; set; }
            public string PlanName { get; set; }
            public string OldExerciseId { get; set; }
            public string NewExerciseId { get; set; }
            public string NewExerciseName { get; set; }
        }
        public class AiRecommendDto
        {
            public long TelegramId { get; set; }
            public string PlanName { get; set; }
        }
    }
}
