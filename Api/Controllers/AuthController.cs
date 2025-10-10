using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.DTOs.Auth;
using Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

public class AuthController : BaseApiController
{
    private readonly UserService _userService;

    public AuthController(UserService userService)
    {
        _userService = userService;
    }

    // ✅ Registro
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var result = await _userService.RegisterAsync(dto);
        return Ok(new { message = result });
    }

    // ✅ Login
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto, CancellationToken ct)
    {
        var result = await _userService.GetTokenAsync(dto, ct);
        if (!result.IsAuthenticated)
            return Unauthorized(result);

        return Ok(result);
    }

    // ✅ Asignar rol
    [HttpPost("add-role")]
    public async Task<IActionResult> AddRole([FromBody] AddRoleDto dto)
    {
        var result = await _userService.AddRoleAsync(dto);
        return Ok(new { message = result });
    }

    // ✅ Refrescar token
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto dto)
    {
        if (string.IsNullOrEmpty(dto.RefreshToken))
            return BadRequest(new { message = "Refresh token es requerido" });

        var result = await _userService.RefreshTokenAsync(dto.RefreshToken);
        if (!result.IsAuthenticated)
            return Unauthorized(result);

        return Ok(result);
    }
}