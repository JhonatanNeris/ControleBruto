using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos;
using ControleBruto.Extensions;
using ControleBruto.Models;
using ControleBruto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]

public class AccountsController : ControllerBase
{

    private ControleBrutoContext _context;
    private IMapper _mapper;
    private AccountService _accountService;

    public AccountsController(ControleBrutoContext context, IMapper mapper, AccountService accountService)
    {
        _context = context;
        _mapper = mapper;
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadAccountDto>>> Get()
    {
        var userId = User.GetUserId();

        var result = await _accountService.GetAllAsync(userId);

        return Ok(result);
    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var result = await _accountService.GetByIdAsync(userId, id);

        return result is null ? NotFound() : Ok(result);

    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
    {
        var userId = User.GetUserId();

        var result = await _accountService.CreateAsync(userId, dto);

        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

    }
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountDto dto)
    {
        var userId = User.GetUserId();

        await _accountService.UpdateAsync(userId, id, dto);

        return NoContent();

    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        await _accountService.DeleteAsync(userId, id);

        return NoContent();
    }
}
