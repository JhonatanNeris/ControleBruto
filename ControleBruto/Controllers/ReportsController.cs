using ControleBruto.Data;
using ControleBruto.Extensions;
using ControleBruto.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]

public class ReportsController : ControllerBase
{
    private readonly ControleBrutoContext _context;

    public ReportsController(ControleBrutoContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetReport([FromQuery] int month, [FromQuery] int year)
    {

        var userId = User.GetUserId();
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        Console.WriteLine(startDate);
        Console.WriteLine(endDate);

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

        return Ok(new
        {
            Period = new { month, year },
            MonthlyIncome = totalIncome,
            MonthlyExpense = totalExpense,
            MonthlyBalance = totalIncome - totalExpense,
            TotalNetWorth = netWorth
        });
    }

    [HttpGet]
    [Route("by-category")]
    public async Task<IActionResult> GetReportByCategory([FromQuery] int month, [FromQuery] int year, [FromQuery] TransactionType type)
    {
        var userId = User.GetUserId();
        var startDate = new DateTime(year, month, 1);
        var endDate = startDate.AddMonths(1);

        // Só faz sentido agrupar por categoria para Income ou Expense
        if (type != TransactionType.Expense && type != TransactionType.Income)
            return BadRequest(new { message = "type deve ser Expense ou Income." });

        var transactions = await _context.Transactions
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

        var categoryIds = transactions
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
        var result = transactions.Select(x => new
        {
            x.CategoryId,
            CategoryName = x.CategoryId.HasValue && categoryMap.ContainsKey(x.CategoryId.Value)
                    ? categoryMap[x.CategoryId.Value]
                    : "Sem categoria",
            x.TotalCents,
            x.Count
        });


        return Ok(new
        {
            Period = new { month, year },    
            Type = type.ToString(),
            Result = result
        });
    }
}
