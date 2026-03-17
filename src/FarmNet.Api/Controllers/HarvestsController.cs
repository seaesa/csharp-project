using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/harvests")]
[Authorize]
public class HarvestsController(IHarvestService harvestService) : ControllerBase
{
    [HttpGet("{batchId:guid}")]
    public async Task<IActionResult> GetByBatch(Guid batchId)
    {
        var harvest = await harvestService.GetByBatchAsync(batchId);
        return harvest == null ? NotFound(new { message = "Chưa có thông tin thu hoạch" }) : Ok(harvest);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,FarmOwner")]
    public async Task<IActionResult> Create([FromBody] TaoThuHoachRequest request)
    {
        var result = await harvestService.CreateAsync(request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Created($"/api/harvests/{request.BatchId}", result.Value);
    }
}
