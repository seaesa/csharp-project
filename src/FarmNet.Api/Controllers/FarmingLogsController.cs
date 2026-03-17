using System.Security.Claims;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/farming-logs")]
[Authorize]
public class FarmingLogsController(IFarmingLogService logService) : ControllerBase
{
    [HttpGet("{batchId:guid}")]
    public async Task<IActionResult> GetByBatch(Guid batchId) =>
        Ok(await logService.GetByBatchAsync(batchId));

    [HttpPost]
    [Authorize(Roles = "Admin,FarmOwner,Worker")]
    public async Task<IActionResult> Create([FromBody] TaoNhatKyRequest request)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier)!;
        var result = await logService.CreateAsync(request, userId);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Created($"/api/farming-logs/{request.BatchId}", result.Value);
    }
}
