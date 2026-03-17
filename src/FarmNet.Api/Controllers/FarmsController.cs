using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/farms")]
[Authorize]
public class FarmsController(IFarmService farmService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await farmService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var farm = await farmService.GetByIdAsync(id);
        return farm == null ? NotFound(new { message = "Không tìm thấy trang trại" }) : Ok(farm);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] TaoFarmRequest request)
    {
        var result = await farmService.CreateAsync(request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Created($"/api/farms/{result.Value.Id}", result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaoFarmRequest request)
    {
        var result = await farmService.UpdateAsync(id, request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Ok(result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await farmService.DeleteAsync(id);
        if (result.IsFailed) return NotFound(new { message = result.Errors.First().Message });
        return NoContent();
    }
}
