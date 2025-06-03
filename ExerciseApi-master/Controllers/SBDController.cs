using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using ExerciseApi.Models;
using ExerciseApi.Data;
using System.Linq;
using ExerciseApi.Body;

namespace ExerciseApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SBDController : ControllerBase
    {
        private readonly AppDbContext _db;
        public SBDController(AppDbContext db)
        {
            _db = db;
        }
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SBDRequestDto dto)
        {
            if (dto == null || dto.userId == 0)
                return BadRequest("userId is required");
            var record = _db.SBDRecords.FirstOrDefault(x => x.UserId == dto.userId);
            if (record == null)
            {
                _db.SBDRecords.Add(new SBDRecord
                {
                    UserId = dto.userId,
                    Squat = dto.squat,
                    Bench = dto.bench,
                    Deadlift = dto.deadlift
                });
            }
            else
            {
                record.Squat = dto.squat;
                record.Bench = dto.bench;
                record.Deadlift = dto.deadlift;
                _db.SBDRecords.Update(record);
            }
            await _db.SaveChangesAsync();
            return Ok();
        }
        [HttpGet("{userId}")]
        public async Task<IActionResult> Get(long userId)
        {
            var sbd = await _db.SBDRecords.FindAsync(userId);
            if (sbd == null)
                return NotFound();
            return Ok(new { squat = sbd.Squat, bench = sbd.Bench, deadlift = sbd.Deadlift });
        }
        [HttpDelete("{userId}")]
        public async Task<IActionResult> Delete(long userId)
        {
            var sbd = await _db.SBDRecords.FindAsync(userId);
            if (sbd != null)
            {
                _db.SBDRecords.Remove(sbd);
                await _db.SaveChangesAsync();
            }
            return Ok();
        }
    }
}
