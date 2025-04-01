using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooklinkBE.API.Controllers;

[ApiController]
[Route("v1/api/households")]
public class HouseholdController(IHouseholdService householdService) : ControllerBase
{
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var location = await householdService.GetHouseholdByIdAsync(id);
        if (location == null) return NotFound();
        return Ok(location);
    }
    
    [HttpGet("user:{userId:guid}")]
    public async Task<ActionResult<IEnumerable<Household>>> GetAllHouseholdsByUserId(Guid userId)
    {
        var estates = await householdService.GetHouseholdsByUserIdAsync(userId);
        return Ok(estates);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody]CreateHouseholdRequest request)
    {
        var householdId = await householdService.CreateHouseholdAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = householdId }, householdId);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateHouseholdRequest request)
    {
        var updated = await householdService.UpdateHouseholdAsync(request);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await householdService.DeleteHouseholdAsync(id);
            return NoContent();
        }
        catch (ArgumentException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
