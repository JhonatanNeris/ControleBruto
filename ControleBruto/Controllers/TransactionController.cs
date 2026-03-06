using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos;
using ControleBruto.Data.Dtos.Transaction;
using ControleBruto.Extensions;
using ControleBruto.Models;
using ControleBruto.Models.Enums;
using ControleBruto.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class TransactionController : ControllerBase
{
    private TransactionService _transactionService;

    public TransactionController(TransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTransactionDto dto)
    {

        var userId = User.GetUserId();

        var result = await _transactionService.CreateAsync(userId, dto);
       
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);

    }
    [HttpPost("transfer")]
    public async Task<IActionResult> CreateTransfer([FromBody] CreateTransferDto dto)
    {

        var userId = User.GetUserId();

        var result = await _transactionService.CreateTransferAsync(userId, dto);

        return CreatedAtAction(nameof(GetById), new { id = result.OutTransaction.Id }, result);

    }


    [HttpGet]
    public async Task<ActionResult<PagedResultDto<ReadTransactionDto>>> Get(TransactionQueryDto queryDto)
    {
        var userId = User.GetUserId();

        var result = await _transactionService.GetAllAsync(userId, queryDto);

        return Ok(result);
    }

    // GET /Transactions/{id}
    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var result = await _transactionService.GetByIdAsync(userId, id);

        return result is null ? NotFound(new { message = "Transação não encontrada." }) : Ok(result);
    }
    

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateTransactionDto dto)
    {
        var userId = User.GetUserId();

        await _transactionService.UpdateAsync(userId, id, dto);

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        await _transactionService.DeleteAsync(userId, id);

        return NoContent();
    }

   

}
