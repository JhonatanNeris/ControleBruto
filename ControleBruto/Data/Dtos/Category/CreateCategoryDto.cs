using ControleBruto.Models;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos.Category;

public class CreateCategoryDto
{
    [Required(ErrorMessage ="O nome da categoria é obrigatório.")]
    public string Name { get; set; }
    [Required(ErrorMessage = "O tipo (Receita ou Despesa) é obrigatório.")]
    public CategoryType Type { get; set; }
    public string Color { get; set; } = "#808080";
}
