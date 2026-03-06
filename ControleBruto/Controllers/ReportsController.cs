using ControleBruto.Data;
using ControleBruto.Extensions;
using ControleBruto.Models.Enums;
using ControleBruto.Services;
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
    private ReportsService _reportService;

    public ReportsController(ControleBrutoContext context, ReportsService reportService)
    {
        _context = context;
        _reportService = reportService;
    }

    [HttpGet]
    public async Task<IActionResult> GetReport([FromQuery] int month, [FromQuery] int year)
    {
        var userId = User.GetUserId();

        var result = await _reportService.GetMonthlyReportAsync(userId, month, year);

        return Ok(result);
    }

    [HttpGet]
    [Route("by-category")]
    public async Task<IActionResult> GetReportByCategory([FromQuery] int month, [FromQuery] int year, [FromQuery] TransactionType type)
    {
        var userId = User.GetUserId();

        var result = await _reportService.GetReportByCategoryAsync(userId, month, year, type);

        return Ok(result);

    }
}
