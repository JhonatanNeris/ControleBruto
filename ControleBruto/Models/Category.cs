using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Models
{
    public class Category
    {
        [Key]
        [Required]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
    }
}
