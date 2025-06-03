using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ExerciseApi.Models;
using ExerciseApi.Data;
using ExerciseApi.Body;

namespace APIprot.Controllers
{
    [ApiController]
    [Route("api/Favorite")]
    public class FavoriteController : ControllerBase
    {
        private readonly AppDbContext _db;
        public FavoriteController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("add")]
        public IActionResult Add([FromBody] FavoriteDto dto)
        {
            if (!_db.FavoriteExercises.Any(x => x.UserId == dto.TelegramId && x.ExerciseId == dto.ExerciseId))
            {
                _db.FavoriteExercises.Add(new FavoriteExercise
                {
                    UserId = dto.TelegramId,
                    ExerciseId = dto.ExerciseId,
                    ExerciseName = dto.ExerciseName
                });
                _db.SaveChanges();
            }
            return Ok();
        }

        [HttpDelete("remove/{telegramId}/{exerciseId}")]
        public ActionResult Remove(long telegramId, string exerciseId)
        {
            var items = _db.FavoriteExercises.Where(x => x.UserId == telegramId && x.ExerciseId == exerciseId).ToList();
            if (items.Count > 0)
            {
                _db.FavoriteExercises.RemoveRange(items);
                _db.SaveChanges();
                return Ok(new { removed = true });
            }
            return Ok(new { removed = false });
        }

        [HttpGet("user/{telegramId}")]
        public ActionResult<List<FavoriteDto>> Get(long telegramId)
        {
            var list = _db.FavoriteExercises.Where(x => x.UserId == telegramId)
                .Select(x => new FavoriteDto { ExerciseId = x.ExerciseId, ExerciseName = x.ExerciseName })
                .ToList();
            return list;
        }
    }
}
