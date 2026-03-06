using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos;
using ControleBruto.Data.Dtos.Transaction;
using ControleBruto.Exceptions;
using ControleBruto.Models;
using ControleBruto.Models.Enums;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Services
{
    public class TransactionService
    {
        private ControleBrutoContext _context;
        private IMapper _mapper;

        public TransactionService(ControleBrutoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReadTransactionDto> CreateAsync(string userId, CreateTransactionDto dto)
        {
            // Validação: somente Expense/Income aqui
            if (dto.Type is TransactionType.TransferIn or TransactionType.TransferOut)
                throw new BusinessRuleException("Use o endpoint /Transactions/transfer para transferências.");

            await ValidateAccountAsync(userId, dto.AccountId);

            if (dto.CategoryId.HasValue)
                await ValidateCategoryAsync(userId, dto.CategoryId.Value);

            var transaction = _mapper.Map<Models.Transaction>(dto);
            
            transaction.UserId = userId;

            _context.Transactions.Add(transaction);

            await _context.SaveChangesAsync();

            return _mapper.Map<ReadTransactionDto>(transaction);

        }

        public async Task<CreateTransferResultDto> CreateTransferAsync(string userId, CreateTransferDto dto)
        {

            if (dto.FromAccountId == dto.ToAccountId)
                throw new BusinessRuleException("A conta de origem e destino não podem ser a mesma.");

            // Validação: ambas as contas pertencem ao usuário
            var accounts = await _context.Accounts
                    .AsNoTracking()
                    .Where(a => (a.Id == dto.FromAccountId || a.Id == dto.ToAccountId) && a.UserId == userId)
                    .Select(a => a.Id)
                    .ToListAsync();

            if (!accounts.Contains(dto.FromAccountId) || !accounts.Contains(dto.ToAccountId))
                throw new BusinessRuleException("Ambas as contas devem pertencer ao usuário.");

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

            return new CreateTransferResultDto
            {
                TransferGroupId = groupId,
                OutTransaction = _mapper.Map<ReadTransactionDto>(outTx),
                InTransaction = _mapper.Map<ReadTransactionDto>(inTx)
            };
        }

        public async Task<PagedResultDto<ReadTransactionDto>> GetAllAsync(string userId, TransactionQueryDto queryDto)
        {
            var skip = queryDto.Skip < 0 ? 0 : queryDto.Skip;
            var take = queryDto.Take < 1 ? 1 : queryDto.Take > 200 ? 200 : queryDto.Take;

            IQueryable<Transaction> query = _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId);

            if (queryDto.From.HasValue)
                query = query.Where(t => t.Date >= queryDto.From.Value);

            if (queryDto.To.HasValue)
                query = query.Where(t => t.Date <= queryDto.To.Value);

            if (queryDto.AccountId.HasValue)
                query = query.Where(t => t.AccountId == queryDto.AccountId.Value);

            if (queryDto.CategoryId.HasValue)
                query = query.Where(t => t.CategoryId == queryDto.CategoryId.Value);

            if (queryDto.Type.HasValue)
                query = query.Where(t => t.Type == queryDto.Type.Value);

            if (!string.IsNullOrWhiteSpace(queryDto.Search))
            {
                var term = queryDto.Search.Trim();
                query = query.Where(t => t.Description.Contains(term));
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(t => t.Date)
                .ThenByDescending(t => t.Id)
                .Include(t => t.Category)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return new PagedResultDto<ReadTransactionDto>
            {
                Total = total,
                Skip = skip,
                Take = take,
                Items = _mapper.Map<List<ReadTransactionDto>>(items)
            };
        }

        public async Task<ReadTransactionDto?> GetByIdAsync(string userId, int id)
        {
            var transaction = await _context.Transactions
                .AsNoTracking()
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id && t.UserId == userId);

            return transaction is null ? null : _mapper.Map<ReadTransactionDto>(transaction);
        }



        public async Task UpdateAsync(string userId, int id, UpdateTransactionDto dto)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null)
                throw new NotFoundException("Transação não encontrada.");

            if (transaction.TransferGroupId.HasValue)
                throw new NotFoundException("Transações de transferência não podem ser editadas. Exclua e crie novamente.");

            await ValidateAccountAsync(userId, dto.AccountId);

            if (dto.CategoryId.HasValue)
                await ValidateCategoryAsync(userId, dto.CategoryId.Value);

            _mapper.Map(dto, transaction);

            await _context.SaveChangesAsync();

        }

        public async Task DeleteAsync(string userId, int id)
        {
            var transaction = await _context.Transactions
                .Where(t => t.Id == id && t.UserId == userId)
                .FirstOrDefaultAsync();

            if (transaction == null)
                throw new NotFoundException("Transação não encontrada.");

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

        }

        private async Task ValidateCategoryAsync(string userId, int categoryId)
        {
            var categoryExists = await _context.Categories
                .AsNoTracking()
                .AnyAsync(c => c.Id == categoryId && c.UserId == userId);

            if (!categoryExists)
                throw new BusinessRuleException("Categoria inválida (não pertence ao usuário).");
        }

        private async Task ValidateAccountAsync(string userId, int accountId)
        {
            var accountExists = await _context.Accounts
                .AsNoTracking()
                .AnyAsync(a => a.Id == accountId && a.UserId == userId);

            if (!accountExists)
                throw new BusinessRuleException("Conta inválida (não pertence ao usuário).");
        }
    }
}
