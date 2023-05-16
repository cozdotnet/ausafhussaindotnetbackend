using Microsoft.AspNetCore.Mvc;

namespace PerCare.Controllers
{
    [Route("pet")]
    [ApiController]
    public class PetController : ControllerBase
    {
        private PetcareContext _context;
        public PetController(PetcareContext context)
        {
            _context = context;
        }
        [HttpGet("getAllPetSchedules/{id}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllSchedules(int id)
        {
            if (_context == null)
            {
                return NotFound("Database not found!");
            }
            var query = from p in _context.Pets
                        join u in _context.Users on p.Userid equals u.Id
                        where u.Id == id
                        select new
                        {
                            p.Petid,
                            p.Name,
                            p.Medicalname,
                            p.Date
                        };
            return query.ToList();
        }
        [HttpPost("createPetSession")]
        public async Task<ActionResult<Pet>> CreateBatch(Pet item)
        {
            _context.Pets.Add(item);
            {
                await _context.SaveChangesAsync();
            }
            return Ok();
        }
        [HttpGet("getUpcomingSession/{id}")]
        public async Task<ActionResult<object>> GetUpcomingSessions(int id)
        {
            if (_context == null)
            {
                return NotFound("Database not found!");
            }
            DateTime currentDate = DateTime.Now.Date;
            int targetUserId = id;

            var nearestDate = (from pet in _context.Pets
                               where pet.Date >= currentDate && pet.Userid == targetUserId
                               orderby pet.Date ascending
                               select new
                               {
                                   pet.Userid,
                                   pet.Name,
                                   pet.Medicalname,
                                   nearest_date = pet.Date
                               }).FirstOrDefault();

            if (nearestDate == null)
            {
                return NotFound("No schedules found for the specified user ID.");
            }
            return Ok(nearestDate);
        }
        [HttpDelete("deletePetSession/{id}")]
        public async Task<IActionResult> DeleteProgram(int id)
        {
            var item = _context.Pets.Find(id);
            if (_context.Pets == null)
            {
                return NotFound($"Code {id} not found!");
            }
            item.Userid = null;
            await _context.SaveChangesAsync();
            _context.Pets.Remove(item);
            await _context.SaveChangesAsync();
            return Ok();
        }
    }
}
