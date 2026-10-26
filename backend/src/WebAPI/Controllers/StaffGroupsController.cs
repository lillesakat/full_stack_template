using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/staff-groups")]
public class StaffGroupsController : ControllerBase
{
    private readonly IStaffGroupService _staffGroupService;

    public StaffGroupsController(IStaffGroupService staffGroupService)
    {
        _staffGroupService = staffGroupService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<StaffGroupDto>>> GetAll()
        => Ok(await _staffGroupService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<StaffGroupDto>> GetById(int id)
        => Ok(await _staffGroupService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<StaffGroupDto>> Create(SaveStaffGroupRequest request)
    {
        var createdStaffGroup = await _staffGroupService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdStaffGroup.Id }, createdStaffGroup);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<StaffGroupDto>> Update(int id, SaveStaffGroupRequest request)
        => Ok(await _staffGroupService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _staffGroupService.DeleteAsync(id);
        return NoContent();
    }
}
