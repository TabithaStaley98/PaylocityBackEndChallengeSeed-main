using Api.ApplicationDbContext;
using Api.Models;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
public class PaychecksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly int yearlyPaycheckNum = 26;
    private readonly decimal MonthlyBaseBenefits = 1000;
    private readonly decimal MonthlyDependantBaseBenefits = 600;
    private readonly decimal MonthlyOlderDependantBenefits = 200;

    public PaychecksController(AppDbContext context) {
        _context = context;
    }
    
    [SwaggerOperation(Summary = "Get paycheck by employee id")]
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<Paycheck>>> CalculatePaycheck(int id)
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

        var paycheck = new Paycheck();
        paycheck.GrossPay = Math.Round(employee.Salary / yearlyPaycheckNum, 2);
        paycheck.BaseBenefits = Math.Round(MonthlyBaseBenefits * 12 / yearlyPaycheckNum, 2);
        
        decimal dependentBenefits = 0;
        DateTime dtCurrent = DateTime.Now;
        foreach(Dependent d in employee.Dependents) {            
            DateTime dtDOB = d.DateOfBirth;
            TimeSpan timeSpan = dtCurrent - dtDOB;
            DateTime age = DateTime.MinValue + timeSpan;
            int years = age.Year - 1;

            if (years > 50) {
                dependentBenefits += MonthlyOlderDependantBenefits;
            }
            dependentBenefits += MonthlyDependantBaseBenefits;
        }
        paycheck.DependentBenefits = Math.Round(dependentBenefits * 12 / yearlyPaycheckNum, 2);
        
        if (employee.Salary > 80000) {
            paycheck.AdditionalBenefits = Math.Round(employee.Salary * (decimal)0.02 / yearlyPaycheckNum, 2);
        }

        paycheck.NetPay = paycheck.GrossPay - paycheck.BaseBenefits - paycheck.DependentBenefits - paycheck.AdditionalBenefits;

        var result = new ApiResponse<Paycheck>
        {
            Data = paycheck,
            Success = true
        };

        return result;
    }
}
