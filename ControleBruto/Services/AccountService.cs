using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos;
using ControleBruto.Data.Dtos.Category;
using ControleBruto.Exceptions;
using ControleBruto.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace ControleBruto.Services;

public class AccountService
{
    private ControleBrutoContext _context;
    private IMapper _mapper;

    public AccountService(ControleBrutoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReadAccountDto>> GetAllAsync(string userId)
    {
        var accounts = await _context.Accounts
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReadAccountDto>>(accounts);
    }
    public async Task<ReadAccountDto?> GetByIdAsync(string userId, int id)
    {
        var account = await _context.Accounts
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Id == id)
            .FirstOrDefaultAsync();

        if (account is null)
        {
            return null;
        }

        return _mapper.Map<ReadAccountDto>(account);

    }

    public async Task<ReadAccountDto> CreateAsync(string userId, CreateAccountDto dto)
    {

        Account account = _mapper.Map<Account>(dto);

        account.UserId = userId;

        _context.Accounts.Add(account);

        await _context.SaveChangesAsync();

        return _mapper.Map<ReadAccountDto>(account);
    }

    public async Task UpdateAsync(string userId, int id, UpdateAccountDto dto)
    {
        var account = await _context.Accounts
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();

        if (account is null)
            throw new NotFoundException("Conta não encontrada.");

        _mapper.Map(dto, account);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string userId, int id)
    {
        var account = await _context.Accounts
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();

        if (account is null)
            throw new NotFoundException("Conta não encontrada.");

        _context.Accounts.Remove(account);

        await _context.SaveChangesAsync();
    }
}
