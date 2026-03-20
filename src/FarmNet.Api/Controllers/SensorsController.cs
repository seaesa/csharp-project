using FarmNet.Api.Models;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/sensors")]
public class SensorsController(ISensorService sensorService) : ControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll([FromQuery] Guid? batchId) =>
        Ok(await sensorService.GetAllAsync(batchId));

    [HttpGet("{id:guid}")]
    [Authorize]
    public async Task<IActionResult> GetById(Guid id)
    {
        var sensor = await sensorService.GetByIdAsync(id);
        return sensor == null ? NotFound(new { message = "Không tìm thấy cảm biến" }) : Ok(sensor);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Create([FromBody] TaoSensorRequest request)
    {
        var result = await sensorService.CreateAsync(request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Created($"/api/sensors/{result.Value.Id}", result.Value);
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await sensorService.DeleteAsync(id);
        if (result.IsFailed) return NotFound(new { message = result.Errors.First().Message });
        return NoContent();
    }

    [HttpPost("data")]
    [AllowAnonymous]
    [Consumes("multipart/form-data")]
    public async Task<IActionResult> RecordData([FromForm] SensorDataFormRequest form)
    {
        byte[]? imageBytes = null;
        if (form.Image is { Length: > 0 })
        {
            using var ms = new MemoryStream();
            await form.Image.CopyToAsync(ms);
            imageBytes = ms.ToArray();
        }

        var request = new SensorDataRequest(
            form.BatchId,
            form.Temperature,
            form.Humidity,
            form.SoilPH,
            form.LightLevel,
            form.SoilMoisture,
            imageBytes
        );

        var result = await sensorService.RecordDataAsync(request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Ok(new { message = "Ghi dữ liệu cảm biến thành công" });
    }

    [HttpGet("data")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetRecentData([FromQuery] int count = 50) =>
        Ok(await sensorService.GetRecentDataAsync(count));

    [HttpGet("data/{batchMaLo}")]
    [Authorize]
    public async Task<IActionResult> GetDataByBatch(string batchMaLo) =>
        Ok(await sensorService.GetDataByBatchAsync(batchMaLo));
}
