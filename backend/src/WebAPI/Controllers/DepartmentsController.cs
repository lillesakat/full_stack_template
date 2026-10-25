using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/departments")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;

    public DepartmentsController(IDepartmentService departmentService)
    {
        _departmentService = departmentService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<DepartmentDto>>> GetAll()

        => Ok(await _departmentService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> GetById(int id)
    => Ok(await _departmentService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<DepartmentDto>> Create([FromBody] SaveDepartmentRequest request)
    {
        var createdDepartment = await _departmentService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdDepartment.Id }, createdDepartment);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<DepartmentDto>> Update(int id, SaveDepartmentRequest request)
        => Ok(await _departmentService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _departmentService.DeleteAsync(id);
        return NoContent();
    }

}
