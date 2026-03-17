using FarmNet.Application.DTOs.Requests;
using FarmNet.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FarmNet.Api.Controllers;

[ApiController]
[Route("api/users")]
[Authorize(Roles = "Admin")]
public class UsersController(IUserService userService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await userService.GetAllAsync());

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var user = await userService.GetByIdAsync(id);
        return user == null ? NotFound(new { message = "Không tìm thấy người dùng" }) : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] TaoNguoiDungRequest request)
    {
        var result = await userService.CreateAsync(request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Created($"/api/users/{result.Value.Id}", result.Value);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] CapNhatNguoiDungRequest request)
    {
        var result = await userService.UpdateAsync(id, request);
        if (result.IsFailed) return BadRequest(new { message = result.Errors.First().Message });
        return Ok(result.Value);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        var result = await userService.DeleteAsync(id);
        if (result.IsFailed) return NotFound(new { message = result.Errors.First().Message });
        return NoContent();
    }
}
