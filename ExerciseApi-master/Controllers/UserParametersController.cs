using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using ExerciseApi.Models;
using ExerciseApi.Data;
using ExerciseApi.Body;

namespace APIprot.Controllers
{
    [ApiController]
    [Route("api/userparameters")]
    public class UserParametersController : ControllerBase
    {
        private readonly AppDbContext _db;
        public UserParametersController(AppDbContext db)
        {
            _db = db;
        }

        [HttpPost("{userId}")]
        public IActionResult Set(long userId, [FromBody] UserParamsDto dto)
        {
            var param = _db.UserParameters.FirstOrDefault(x => x.UserId == userId);
            if (param == null)
            {
                _db.UserParameters.Add(new UserParameter { UserId = userId, Height = dto.Height, Weight = dto.Weight });
            }
            else
            {
                param.Height = dto.Height;
                param.Weight = dto.Weight;
                _db.UserParameters.Update(param);
            }
            _db.SaveChanges();
            return Ok();
        }

        [HttpGet("{userId}")]
        public ActionResult<UserParamsDto> Get(long userId)
        {
            var param = _db.UserParameters.FirstOrDefault(x => x.UserId == userId);
            if (param != null)
                return new UserParamsDto { Height = param.Height, Weight = param.Weight };
            return NotFound();
        }

        [HttpDelete("{userId}")]
        public IActionResult Delete(long userId)
        {
            var param = _db.UserParameters.FirstOrDefault(x => x.UserId == userId);
            if (param != null)
            {
                _db.UserParameters.Remove(param);
                _db.SaveChanges();
            }
            return Ok();
        }
    }
}
