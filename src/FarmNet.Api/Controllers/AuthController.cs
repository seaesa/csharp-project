using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var result = await authService.LoginAsync(request);
        if (result.IsFailed)
            return Unauthorized(new { message = result.Errors.First().Message });
        return Ok(result.Value);
    }
}
