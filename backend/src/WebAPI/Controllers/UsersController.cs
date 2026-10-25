using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/users")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;

    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserDto>>> GetAll()

        => Ok(await _userService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserDto>> GetById(int id)
    => Ok(await _userService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<UserDto>> Create([FromBody] SaveUserRequest request)
    {
        var createdUser = await _userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdUser.Id }, createdUser);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<UserDto>> Update(int id, SaveUserRequest request)
        => Ok(await _userService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _userService.DeleteAsync(id);
        return NoContent();
    }

}
