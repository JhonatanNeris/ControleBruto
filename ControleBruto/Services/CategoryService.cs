using AutoMapper;
using ControleBruto.Data;
using ControleBruto.Data.Dtos.Category;
using ControleBruto.Exceptions;
using ControleBruto.Models;
using Microsoft.EntityFrameworkCore;

namespace ControleBruto.Services;

public class CategoryService
{
    private ControleBrutoContext _context;
    private IMapper _mapper;

    public CategoryService(ControleBrutoContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ReadCategoryDto>> GetAllAsync(string userId)
    {
        var categories = await _context.Categories
            .AsNoTracking()
            .Where(c => c.UserId == userId)
            .ToListAsync();

        return _mapper.Map<IEnumerable<ReadCategoryDto>>(categories);
    }
    public async Task<ReadCategoryDto?> GetByIdAsync(string userId, int id)
    {
        var category = await _context.Categories
            .AsNoTracking()
            .Where(a => a.UserId == userId && a.Id == id)
            .FirstOrDefaultAsync();

        if (category is null)
        {
            return null;
        }

        return _mapper.Map<ReadCategoryDto>(category);

    }

    public async Task<ReadCategoryDto> CreateAsync(string userId, CreateCategoryDto dto)
    {

        Category category = _mapper.Map<Category>(dto);

        category.UserId = userId;

        _context.Categories.Add(category);

        await _context.SaveChangesAsync();

        return _mapper.Map<ReadCategoryDto>(category);
    }

    public async Task UpdateAsync(string userId, int id, UpdateCategoryDto dto)
    {
        var category = await _context.Categories
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();

        if (category is null)
            throw new NotFoundException("Categoria não encontrada.");

        _mapper.Map(dto, category);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(string userId, int id)
    {
        var category = await _context.Categories
            .Where(c => c.Id == id && c.UserId == userId)
            .FirstOrDefaultAsync();

        if (category is null)
            throw new NotFoundException("Categoria não encontrada.");

        _context.Categories.Remove(category);

        await _context.SaveChangesAsync();
    }

}
