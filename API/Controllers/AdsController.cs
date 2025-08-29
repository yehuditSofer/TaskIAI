using Common.InterfacesBL;
using Common.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ApiController]
[Route("api/[controller]")]
public class AdsController : ControllerBase
{
    private readonly IAdService _service;
    public AdsController(IAdService service) { _service = service; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ad>>> Get([FromQuery] string? q, [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] double? radiusKm, [FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var ads = await _service.GetAllAsync(q, lat, lng, radiusKm);

        // כאן החיתוך של paging
        var paged = ads
            .Skip(skip)
            .Take(take);

        return Ok(paged);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Ad>> GetById(Guid id)
    {
        var ad = await _service.GetByIdAsync(id);
        if (ad is null) return NotFound();
        return Ok(ad);
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Ad>> Create([FromBody] Ad ad)
    {
        try
        {
            var created = await _service.CreateAsync(ad, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Ad>> Update(Guid id, [FromBody] Ad ad)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, ad, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            if (updated != null)
                return Ok(updated);
            return NotFound();
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [Authorize]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

}