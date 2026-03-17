using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using FarmNet.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/batches")]
[Authorize]
public class BatchesController(IBatchService batchService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll([FromQuery] Guid? farmId) =>
        Ok(await batchService.GetAllAsync(farmId));

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var batch = await batchService.GetByIdAsync(id);
        return batch == null ? NotFound(new { message = "Không tìm thấy lô sản phẩm" }) : Ok(batch);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,FarmOwner")]
    public async Task<IActionResult> Create([FromBody] TaoBatchRequest request)
    {
        var result = await batchService.CreateAsync(request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Created($"/api/batches/{result.Value.Id}", result.Value);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin,FarmOwner")]
    public async Task<IActionResult> Update(Guid id, [FromBody] TaoBatchRequest request)
    {
        var result = await batchService.UpdateAsync(id, request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Ok(result.Value);
    }

    [HttpPut("{id:guid}/status")]
    [Authorize(Roles = "Admin,FarmOwner")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] CapNhatTrangThaiBatchRequest request)
    {
        var result = await batchService.UpdateStatusAsync(id, request.TrangThai);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return NoContent();
    }

    [HttpGet("{id:guid}/blockchain")]
    public async Task<IActionResult> GetBlockchainRecords(Guid id) =>
        Ok(await batchService.GetBlockchainRecordsAsync(id));

    [HttpGet("{id:guid}/qr")]
    [Authorize(Roles = "Admin,FarmOwner")]
    public async Task<IActionResult> GetQrCode(Guid id)
    {
        var base64 = await batchService.GenerateQrCodeAsync(id);
        if (string.IsNullOrEmpty(base64)) return NotFound(new { message = "Không tìm thấy lô sản phẩm" });
        return Ok(new { qrBase64 = base64 });
    }
}
