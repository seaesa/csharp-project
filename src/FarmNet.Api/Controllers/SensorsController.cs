using FarmNet.Api.Models;
using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

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
    [Consumes("application/json")]
    public async Task<IActionResult> RecordData([FromBody] SensorDataIotRequest iot)
    {
        DateTime timeUtc;
        try
        {
            var dt = DateTime.ParseExact(iot.Time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
            timeUtc = DateTime.SpecifyKind(dt, DateTimeKind.Utc);
        }
        catch
        {
            return BadRequest(new { message = "time không đúng định dạng 'yyyy-MM-dd HH:mm:ss' (UTC)" });
        }

        // IoT spec: rain = 1 => không mưa, rain = 0 => mưa
        var coMua = iot.Rain == 0;
        var bomBat = iot.Pump != 0;

        var request = new SensorDataRequest(
            iot.DeviceId,
            iot.BatchId,
            timeUtc,
            iot.Temp,
            iot.Hum,
            coMua,
            iot.Water,
            iot.Gas,
            bomBat
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
