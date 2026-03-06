using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos.Report;
using ControleBruto.Exceptions;
using ControleBruto.Models;
using ControleBruto.Models.Enums;
using ControleBruto.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Services
{
    public class ReportsService : IReportService
    {
        private ControleBrutoContext _context;
        private IMapper _mapper;

        public ReportsService(ControleBrutoContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<ReadMonthlyReportDto> GetMonthlyReportAsync(string userId, int month, int year)
        {
            ValidateMonthAndYear(month, year);

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            var transactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date < endDate)
                .ToListAsync();

            //Soma de receitas e despesas
            var totalIncome = transactions
                .Where(t => t.Type == TransactionType.Income)
                .Sum(t => t.AmountCents);

            var totalExpense = transactions
                .Where(t => t.Type == TransactionType.Expense)
                .Sum(t => t.AmountCents);

            // Saldo total de todas as contas (Patrimônio Líquido) até hoje
            var netWorth = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId)
                .SumAsync(t => (t.Type == TransactionType.Income ? t.AmountCents :
                    t.Type == TransactionType.Expense ? -t.AmountCents : 0));

            return new ReadMonthlyReportDto
            {
                Period = new ReportPeriodDto
                {
                    Month = month,
                    Year = year
                },
                TransactionsCount = transactions.Count,
                MonthlyIncome = totalIncome,
                MonthlyExpense = totalExpense,
                MonthlyBalance = totalIncome - totalExpense,
                TotalNetWorth = netWorth
            };
        }

        public async Task<ReadCategoryReportDto> GetReportByCategoryAsync(string userId, int month, int year, TransactionType type)
        {

            ValidateMonthAndYear(month, year);

            var startDate = new DateTime(year, month, 1);
            var endDate = startDate.AddMonths(1);

            // Só faz sentido agrupar por categoria para Income ou Expense
            if (type != TransactionType.Expense && type != TransactionType.Income)
                throw new BusinessRuleException("type deve ser Expense ou Income.");

            var groupedTransactions = await _context.Transactions
                .AsNoTracking()
                .Where(t => t.UserId == userId && t.Date >= startDate && t.Date < endDate && t.Type == type)
                .GroupBy(t => t.CategoryId)
                .Select(g => new
                {
                    CategoryId = g.Key,
                    TotalCents = g.Sum(x => x.AmountCents),
                    Count = g.Count()
                })
                .ToListAsync();

            var categoryIds = groupedTransactions
              .Where(x => x.CategoryId.HasValue)
              .Select(x => x.CategoryId!.Value)
              .Distinct()
              .ToList();

            var categoryMap = await _context.Categories
             .AsNoTracking()
             .Where(c => c.UserId == userId && categoryIds.Contains(c.Id))
             .Select(c => new { c.Id, c.Name })
             .ToDictionaryAsync(x => x.Id, x => x.Name);


            // 3) Monta resposta final com nomes
            var result = groupedTransactions.Select(x => new ReadCategoryReportItemDto
            {
                CategoryId = x.CategoryId,
                CategoryName = x.CategoryId.HasValue && categoryMap.ContainsKey(x.CategoryId.Value)
                        ? categoryMap[x.CategoryId.Value]
                        : "Sem categoria",
                TotalCents = x.TotalCents,
                Count = x.Count
            });


            return new ReadCategoryReportDto
            {
                Period = new ReportPeriodDto
                {
                    Month = month,
                    Year = year
                },
                Type = type.ToString(),
                Result = result
            };

        }

        private static void ValidateMonthAndYear(int month, int year)
        {
            if (month < 1 || month > 12)
                throw new BusinessRuleException("Mês inválido. Informe um valor entre 1 e 12.");

            if (year < 1)
                throw new BusinessRuleException("Ano inválido.");
        }


    }
}
