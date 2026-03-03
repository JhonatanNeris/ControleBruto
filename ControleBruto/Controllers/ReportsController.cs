using ControleBruto.Data;
using ControleBruto.Extensions;
using ControleBruto.Models.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
            .SumAsync(t => (t.Type == TransactionType.Income ?  t.AmountCents : 
                t.Type == TransactionType.Expense ? - t.AmountCents : 0));

        return Ok(new
        {
            Period = new { month, year },
            MonthlyIncome = totalIncome,
            MonthlyExpense = totalExpense,
            MonthlyBalance = totalIncome - totalExpense,
            TotalNetWorth = netWorth
        });      
    }
}
