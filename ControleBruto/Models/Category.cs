using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Name { get; set; }
        [Required]
        public CategoryType Type { get; set; }
        public string Color { get; set; } = "#808080";
        public User? User { get; set; }
    }

    public enum CategoryType
    {
        Expense = 0, 
        Income = 1  
    }
}
