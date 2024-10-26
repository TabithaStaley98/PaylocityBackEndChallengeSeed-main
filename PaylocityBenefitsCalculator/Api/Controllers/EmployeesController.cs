using Api.ApplicationDbContext;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class EmployeesController : ControllerBase
{
    private readonly AppDbContext _context;

    public EmployeesController(AppDbContext context) {
        _context = context;
    }
    
    [SwaggerOperation(Summary = "Get employee by id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Employee>>> GetEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);

        if(employee == null)
        {
            return NotFound();
        }

        var dependents = from d in _context.Dependents
                            where d.EmployeeId.Equals(employee.Id)
                            select d;
        employee.Dependents = dependents.ToList();

        var result = new ApiResponse<Employee>
        {
            Data = employee,
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Get all employees")]
    [HttpGet("")]
    public async Task<ActionResult<ApiResponse<List<Employee>>>> GetAllEmployees()
    {
        //task: use a more realistic production approach
        // Added SQLite db for storing of employee and dependant data
        var employees = await _context.Employees.ToListAsync();
        for (int i = 0; i < employees.Count; i++) {
            var employee = employees[i];
            var dependents = from d in _context.Dependents
                                where d.EmployeeId.Equals(employee.Id)
                                select d;
            employee.Dependents = dependents.ToList();
        }

        var result = new ApiResponse<List<Employee>>
        {
            Data = employees,
            Success = true
        };

        return result;
    }

    [SwaggerOperation(Summary = "Add employee")]
    [HttpPost]
    public async Task<ActionResult<Employee>> PostEmployee(Employee employee)
    {
        _context.Employees.Add(employee);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetEmployee), new { id = employee.Id }, employee);
    }

    [SwaggerOperation(Summary = "Update employee")]
    [HttpPut("{id}")]
    public async Task<IActionResult> PutEmployee(int id, Employee employee)
    {
        if(id != employee.Id)
        {
            return BadRequest();
        }

        _context.Entry(employee).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch(DbUpdateConcurrencyException)
        {
            if(!EmployeeExists(id))
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

    [SwaggerOperation(Summary = "Delete employee")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEmployee(int id)
    {
        var employee = await _context.Employees.FindAsync(id);
        if(employee == null)
        {
            return NotFound();
        }

        _context.Employees.Remove(employee);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool EmployeeExists(int id)
    {
        return _context.Employees.Any(e => e.Id == id);
    }
}
