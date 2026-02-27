using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos;
using ControleBruto.Data.Dtos.Category;
using ControleBruto.Extensions;
using ControleBruto.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]

public class CategoryController : ControllerBase
{
    private ControleBrutoContext _context;
    private IMapper _mapper;

    public CategoryController(ControleBrutoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadCategoryDto>>> Get()
    {
        var userId = User.GetUserId();

        var categories = await _context.Categories
            .AsNoTracking()
            .Where(conta => conta.UserId == userId)
            .ToListAsync();

        return _mapper.Map<List<ReadCategoryDto>>(categories);

    }

    [HttpGet]
    [Route("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var userId = User.GetUserId();

        var category = await _context.Categories
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Id == id)
            .FirstOrDefaultAsync();

        if (category != null)
        {
            ReadCategoryDto categoryDto = _mapper.Map<ReadCategoryDto>(category);
            return Ok(categoryDto);
        }

        return NotFound();
    }
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryDto dto)
    {
        var userId = User.GetUserId();

        Category category = _mapper.Map<Category>(dto);

        category.UserId = userId;

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        var readDto = _mapper.Map<ReadCategoryDto>(category);

        return CreatedAtAction(nameof(GetById), new { id = category.Id }, readDto);
    }
    [HttpPut]
    [Route("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCategoryDto dto)
    {
        var userId = User.GetUserId();

        var category = await _context.Categories
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();

        if (category == null) return NotFound(new { message = "Categoria não encontrada." });

        _mapper.Map(dto, category);

        await _context.SaveChangesAsync();

        return NoContent();

    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var userId = User.GetUserId();

        var category = await _context.Categories
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();

        if (category == null) return NotFound(new { message = "Categoria não encontrada." });

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();

        return NoContent();
    }




}
