using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicAPI.Data;
using VetClinicAPI.Models;

namespace VetClinicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetOwnersController : ControllerBase
    {
        private readonly AppDbContext _context;

        public PetOwnersController(AppDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetOwner>>> GetPetOwners()
        {
            return await _context.PetOwners.ToListAsync();
        }

       
        [HttpGet("{id}")]
        public async Task<ActionResult<PetOwner>> GetPetOwner(int id)
        {
            var petOwner = await _context.PetOwners.FindAsync(id);

            if (petOwner == null)
            {
                return NotFound();
            }

            return petOwner;
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPetOwner(int id, PetOwner petOwner)
        {
            if (id != petOwner.Id)
            {
                return BadRequest();
            }

            _context.Entry(petOwner).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PetOwnerExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        
        [HttpPost]
        public async Task<ActionResult<PetOwner>> PostPetOwner(PetOwner petOwner)
        {
            _context.PetOwners.Add(petOwner);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPetOwner", new { id = petOwner.Id }, petOwner);
        }

       
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetOwner(int id)
        {
            var petOwner = await _context.PetOwners.FindAsync(id);
            if (petOwner == null)
            {
                return NotFound();
            }

            _context.PetOwners.Remove(petOwner);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool PetOwnerExists(int id)
        {
            return _context.PetOwners.Any(e => e.Id == id);
        }
    }
}