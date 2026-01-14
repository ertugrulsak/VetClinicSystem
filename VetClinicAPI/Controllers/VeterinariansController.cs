using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VetClinicAPI.Data;
using VetClinicAPI.Models;

namespace VetClinicAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VeterinariansController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VeterinariansController(AppDbContext context)
        {
            _context = context;
        }

       
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Veterinarian>>> GetVeterinarians()
        {
            return await _context.Veterinarians.ToListAsync();
        }

    
        [HttpGet("{id}")]
        public async Task<ActionResult<Veterinarian>> GetVeterinarian(int id)
        {
            var veterinarian = await _context.Veterinarians.FindAsync(id);

            if (veterinarian == null)
            {
                return NotFound();
            }

            return veterinarian;
        }

       
        [HttpPut("{id}")]
        public async Task<IActionResult> PutVeterinarian(int id, Veterinarian veterinarian)
        {
            if (id != veterinarian.Id)
            {
                return BadRequest();
            }

            _context.Entry(veterinarian).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VeterinarianExists(id))
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
        public async Task<ActionResult<Veterinarian>> PostVeterinarian(Veterinarian veterinarian)
        {
            _context.Veterinarians.Add(veterinarian);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetVeterinarian", new { id = veterinarian.Id }, veterinarian);
        }

      
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVeterinarian(int id)
        {
            var veterinarian = await _context.Veterinarians.FindAsync(id);
            if (veterinarian == null)
            {
                return NotFound();
            }

            _context.Veterinarians.Remove(veterinarian);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool VeterinarianExists(int id)
        {
            return _context.Veterinarians.Any(e => e.Id == id);
        }
    }
}