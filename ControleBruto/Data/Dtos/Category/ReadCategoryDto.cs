using ControleBruto.Models;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos.Category
{
    public class ReadCategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public CategoryType Type { get; set; }
        public string Color { get; set; }
    }
}
