using BooklinkBE.API.Services.Interfaces;
using BooklinkBE.Data.Models;
using Microsoft.AspNetCore.Mvc;

namespace BooklinkBE.API.Controllers;

[ApiController]
[Route("v1/api/[controller]")]
public class RealEstateController(IRealEstateService realEstateService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(CreateRealEstateRequest request)
    {
        var locationId = await realEstateService.CreateRealEstate(request);
        return CreatedAtAction(nameof(GetById), new { id = locationId }, locationId);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var location = await realEstateService.GetRealEstatesById(id);
        if (location == null)
            return NotFound();

        return Ok(location);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Edit(Guid id, UpdateRealEstateRequest request)
    {
        if (id != request.Id)
            return BadRequest("ID mismatch.");

        var updated = await realEstateService.EditRealEstate(request);
        if (!updated)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            await realEstateService.DeleteRealEstate(id);
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
