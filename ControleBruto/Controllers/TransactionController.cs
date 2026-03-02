using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos.Transaction;
using ControleBruto.Extensions;
using ControleBruto.Models;
using ControleBruto.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private ControleBrutoContext _context;
    private IMapper _mapper;

    public TransactionController(ControleBrutoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {

        var userId = User.GetUserId();

        // Validação: somente Expense/Income aqui
        if (dto.Type is TransactionType.TransferIn or TransactionType.TransferOut)
            return BadRequest(new { message = "Use o endpoint /Transactions/transfer para transferências." });

        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == dto.CategoryId.Value && c.UserId == userId);

            if (!categoryExists)
                return BadRequest(new { message = "Categoria inválida (não pertence ao usuário)." });
        }

        // Validação: conta pertence ao usuário
        var accountExists = await _context.Accounts
            .AsNoTracking()
            .AnyAsync(a => a.Id == dto.AccountId && a.UserId == userId);

        if (!accountExists)
            return BadRequest(new { message = "Conta inválida (não pertence ao usuário)." });

        var transaction = _mapper.Map<Transaction>(dto);
        transaction.UserId = userId;

        _context.Transactions.Add(transaction);
        await _context.SaveChangesAsync();

        var read = _mapper.Map<ReadTransactionDto>(transaction);
        return CreatedAtAction(nameof(GetById), new { id = transaction.Id }, read);
        //return Ok(read);

    }
    [HttpPost("transfer")]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
    {

        var userId = User.GetUserId();

        if (dto.FromAccountId == dto.ToAccountId)
            return BadRequest(new { message = "A conta de origem e destino não podem ser a mesma." });


        // Validação: ambas as contas pertencem ao usuário
        var accounts = await _context.Accounts
                .AsNoTracking()
                .Where(a => (a.Id == dto.FromAccountId || a.Id == dto.ToAccountId) && a.UserId == userId)
                .Select(a => a.Id)
                .ToListAsync();

        if (!accounts.Contains(dto.FromAccountId) || !accounts.Contains(dto.ToAccountId))
            return BadRequest(new { message = "Ambas as contas devem pertencer ao usuário." });

        var groupId = Guid.NewGuid();
        var description = string.IsNullOrWhiteSpace(dto.Description) ? "Transferência" : dto.Description.Trim();

        var outTx = new Transaction
        {
            UserId = userId,
            Description = description,
            AccountId = dto.FromAccountId,
            Type = TransactionType.TransferOut,
            CategoryId = null,
            AmountCents = dto.AmountCents,
            Date = dto.Date,
            TransferGroupId = groupId
        };

        var inTx = new Transaction
        {
            UserId = userId,
            Description = description,
            AccountId = dto.ToAccountId,
            Type = TransactionType.TransferIn,
            CategoryId = null,
            AmountCents = dto.AmountCents,
            Date = dto.Date,
            TransferGroupId = groupId
        };

        await using var trx = await _context.Database.BeginTransactionAsync();

        _context.Transactions.AddRange(outTx, inTx);
        await _context.SaveChangesAsync();

        await trx.CommitAsync();

        return Ok(new
        {
            transferGroupId = groupId,
            outTransaction = _mapper.Map<ReadTransactionDto>(outTx),
            inTransaction = _mapper.Map<ReadTransactionDto>(inTx)
        });
    }


    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] DateTime? from,
        [FromQuery] DateTime? to,
        [FromQuery] int? accountId,
        [FromQuery] int? categoryId,
        [FromQuery] TransactionType? type,
        [FromQuery] string? search,
        [FromQuery] int skip = 0,
        [FromQuery] int take = 10
    )
    {
        var userId = User.GetUserId();

        // limites de paginação (evita abuso)
        if (take < 1) take = 1;
        if (take > 200) take = 200;
        if (skip < 0) skip = 0;

        IQueryable<Transaction> query = _context.Transactions
            .AsNoTracking()
            .Where(t => t.UserId == userId);

        // filtro por período
        if (from.HasValue)
            query = query.Where(t => t.Date >= from.Value);

        if (to.HasValue)
            query = query.Where(t => t.Date <= to.Value);

        // filtro por conta
        if (accountId.HasValue)
            query = query.Where(t => t.AccountId == accountId.Value);

        // filtro por categoria
        // (se vier categoryId, normalmente faz sentido ignorar transfers, mas não é obrigatório)
        if (categoryId.HasValue)
            query = query.Where(t => t.CategoryId == categoryId.Value);

        // filtro por tipo
        if (type.HasValue)
            query = query.Where(t => t.Type == type.Value);

        // busca na descrição
        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim();
            query = query.Where(t => t.Description.Contains(term));
        }

        // total para paginação
        var total = await query.CountAsync();

        // paginação + ordenação
        var items = await query
            .OrderByDescending(t => t.Date)
            .ThenByDescending(t => t.Id)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return Ok(new
        {
            total,
            skip,
            take,
            items = _mapper.Map<List<ReadTransactionDto>>(items)
        });
    }

    // GET /Transactions/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var transaction = await _context.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

        if (transaction == null)
            return NotFound(new { message = "Transação não encontrada." });

        return Ok(_mapper.Map<ReadTransactionDto>(transaction));
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

        if (transaction == null)
            return NotFound(new { message = "Transação não encontrada." });

        if (transaction.TransferGroupId.HasValue)
        {
            var relatedTransactions = await _context.Transactions
                .Where(t => t.TransferGroupId == transaction.TransferGroupId && t.UserId == userId)
                .ToListAsync();

            _context.Transactions.RemoveRange(relatedTransactions);
        }

        else
        {
            _context.Transactions.Remove(transaction);
        }

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPut("{id:int}")]

    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionDto dto)
    {
        var userId = User.GetUserId();

        var transaction = await _context.Transactions
            .Where(t => t.Id == id && t.UserId == userId)
            .FirstOrDefaultAsync();

        if (transaction == null)
            return NotFound(new { message = "Transação não encontrada." });

        if (transaction.TransferGroupId.HasValue)
            return BadRequest(new { message = "Transações de transferência não podem ser editadas. Exclua e crie novamente." });

        if (dto.CategoryId.HasValue)
        {
            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == dto.CategoryId.Value && c.UserId == userId);
            if (!categoryExists)
                return BadRequest(new { message = "Categoria inválida (não pertence ao usuário)." });
        }
        // Validação: conta pertence ao usuário
        var accountExists = await _context.Accounts
            .AsNoTracking()
            .AnyAsync(a => a.Id == dto.AccountId && a.UserId == userId);

        if (!accountExists)
            return BadRequest(new { message = "Conta inválida (não pertence ao usuário)." });

        _mapper.Map(dto, transaction);

        await _context.SaveChangesAsync();

        return NoContent();
    }
}
