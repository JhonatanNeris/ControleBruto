using ControleBruto.Models;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos.Category;

public class UpdateCategoryDto
{
    [Required]
    [MaxLength(50)]
    public string Name { get; set; }
    [Required(ErrorMessage = "O tipo da categoria é obrigatório")]
    public CategoryType Type { get; set; }
    public string Color { get; set; }
}
