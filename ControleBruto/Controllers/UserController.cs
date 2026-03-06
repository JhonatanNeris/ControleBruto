using AutoMapper;
using ControleBruto.Data.Dtos;
using ControleBruto.Models;
using ControleBruto.Services;
using Microsoft.AspNetCore.Mvc;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]

public class UserController : ControllerBase
{

    private UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> CreateUser(CreateUserDto dto)
    {
        await _userService.Register(dto);
        return Ok("Usuário cadastrado");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginUserDto dto)
    {
        var token = await _userService.Login(dto);
        return Ok(token);
    }
}
