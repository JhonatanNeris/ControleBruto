using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos;
using ControleBruto.Extensions;
using ControleBruto.Models;
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

    public AccountsController(ControleBrutoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadAccountDto>>> Get()
    {
        var userId = User.GetUserId();

        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(conta => conta.UserId == userId)
            .ToListAsync();

        return _mapper.Map<List<ReadAccountDto>>(accounts);

    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var account = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Id == id)
            .FirstOrDefaultAsync();

        if (account != null)
        {
            ReadAccountDto accountDto = _mapper.Map<ReadAccountDto>(account);
            return Ok(accountDto);
        }

        return NotFound();
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateAccountDto dto)
    {
        var userId = User.GetUserId();

        Account account = _mapper.Map<Account>(dto);

        account.UserId = userId;

        _context.Accounts.Add(account);

        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById),account);
    }
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAccountDto dto)
    {
        var userId = User.GetUserId();

        var account = await _context.Accounts
            .Where(conta => conta.Id == id && conta.UserId == userId)
            .FirstOrDefaultAsync();

        if (account == null) return NotFound(new { message = "Conta não encontrada." });

        _mapper.Map(dto, account);

        await _context.SaveChangesAsync();

        return NoContent();

    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var account = await _context.Accounts
            .Where(conta => conta.Id == id && conta.UserId == userId)
            .FirstOrDefaultAsync();

        if (account == null) return NotFound(new { message = "Conta não encontrada." });

        _context.Accounts.Remove(account);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
