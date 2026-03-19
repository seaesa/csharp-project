using FarmNet.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/public")]
public class PublicController(ITraceabilityService traceabilityService) : ControllerBase
{
    [HttpGet("trace/{batchMaLo}")]
    public async Task<IActionResult> Trace(string batchMaLo)
    {
        var result = await traceabilityService.GetTraceAsync(batchMaLo);
        return result == null ? NotFound(new { message = "Không tìm thấy thông tin lô sản phẩm" }) : Ok(result);
    }

    [HttpGet("trace/{batchMaLo}/verify")]
    public async Task<IActionResult> Verify(string batchMaLo)
    {
        var result = await traceabilityService.VerifyAsync(batchMaLo);
        return result == null ? NotFound(new { message = "Không tìm thấy thông tin lô sản phẩm" }) : Ok(result);
    }
}
