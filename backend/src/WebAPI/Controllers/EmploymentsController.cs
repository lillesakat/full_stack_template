using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/employments")]
public class EmploymentsController : ControllerBase
{
    private readonly IEmploymentService _employmentService;

    public EmploymentsController(IEmploymentService employmentService)
    {
        _employmentService = employmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<EmploymentDto>>> GetAll()
        => Ok(await _employmentService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<EmploymentDto>> GetById(int id)
        => Ok(await _employmentService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<EmploymentDto>> Create(SaveEmploymentRequest request)
    {
        var createdEmployment = await _employmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdEmployment.Id }, createdEmployment);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<EmploymentDto>> Update(int id, SaveEmploymentRequest request)
        => Ok(await _employmentService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _employmentService.DeleteAsync(id);
        return NoContent();
    }
}
