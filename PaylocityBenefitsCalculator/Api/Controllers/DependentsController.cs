using Api.ApplicationDbContext;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class DependentsController : ControllerBase
{
    private readonly AppDbContext _context;

    public DependentsController(AppDbContext context) {
        _context = context;
    }
    
    [SwaggerOperation(Summary = "Get dependent by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Dependent>>> GetDependent(int id)
    {
        var dependent = await _context.Dependents.FindAsync(id);

        if(dependent == null)
        {
            return NotFound();
        }

        var result = new ApiResponse<Dependent>
        {
            Data = dependent,
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Get all dependents")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<Dependent>>>> GetAllDependents()
    {
        var dependents = await _context.Dependents.ToListAsync();

        var result = new ApiResponse<List<Dependent>>
        {
            Data = dependents,
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Add dependent")]
    [HttpPost]
    public async Task<ActionResult<Dependent>> PostDependent(Dependent dependent)
    {
        _context.Dependents.Add(dependent);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetDependent), new { id = dependent.Id }, dependent);
    }

    [SwaggerOperation(Summary = "Update dependent")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutDependent(int id, Dependent dependent)
    {
        if(id != dependent.Id)
        {
            return BadRequest();
        }

        _context.Entry(dependent).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if(!DependentExists(id))
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

    [SwaggerOperation(Summary = "Delete dependent")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDependent(int id)
    {
        var dependent = await _context.Dependents.FindAsync(id);
        if(dependent == null)
        {
            return NotFound();
        }

        _context.Dependents.Remove(dependent);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool DependentExists(int id)
    {
        return _context.Dependents.Any(e => e.Id == id);
    }
}
