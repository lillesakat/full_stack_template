using Application.DTOs;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Controllers;

[ApiController]
[Route("api/periods")]
public class PeriodsController : ControllerBase
{
    private readonly IPeriodService _periodService;

    public PeriodsController(IPeriodService periodService)
    {
        _periodService = periodService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PeriodDto>>> GetAll()

        => Ok(await _periodService.GetAllAsync());

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PeriodDto>> GetById(int id)
    => Ok(await _periodService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<PeriodDto>> Create([FromBody] SavePeriodRequest request)
    {
        var createdPeriod = await _periodService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = createdPeriod.Id }, createdPeriod);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<PeriodDto>> Update(int id, SavePeriodRequest request)
        => Ok(await _periodService.UpdateAsync(id, request));

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _periodService.DeleteAsync(id);
        return NoContent();
    }

}
