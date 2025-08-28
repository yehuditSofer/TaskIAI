using Microsoft.AspNetCore.Mvc;
using Common.InterfacesBL;
using Common.Models;

namespace API;

[ApiController]
[Route("api/[controller]")]
public class AdsController : ControllerBase
{
    private readonly IAdService _service;
    public AdsController(IAdService service) { _service = service; }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Ad>>> Get([FromQuery] string? q, [FromQuery] double? lat, [FromQuery] double? lng, [FromQuery] double? radiusKm)
    {
        var ads = await _service.GetAllAsync(q, lat, lng, radiusKm);
        return Ok(ads);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<Ad>> GetById(Guid id)
    {
        var ad = await _service.GetByIdAsync(id);
        if (ad is null) return NotFound();
        return Ok(ad);
    }

    private string CurrentUserID()
    {
        return User?.Claims.FirstOrDefault(c => c.Type == "email")?.Value ?? string.Empty;
    }

    [HttpPost]
    public async Task<ActionResult<Ad>> Create([FromBody] Ad ad)
    {
        try
        {
            var created = await _service.CreateAsync(ad, CurrentUserID());
            return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
        }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<Ad>> Update(Guid id, [FromBody] Ad ad)
    {
        try
        {
            var updated = await _service.UpdateAsync(id, ad, CurrentUserID());
            if (updated is null) return NotFound();
            return Ok(updated);
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
        catch (ArgumentException ex) { return BadRequest(ex.Message); }
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        try
        {
            var ok = await _service.DeleteAsync(id, CurrentUserID());
            if (!ok) return NotFound();
            return NoContent();
        }
        catch (UnauthorizedAccessException ex) { return Forbid(ex.Message); }
    }

}