using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Models
{
    public class Account
    {
        [Required]
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public double? InitialBalance { get; set; }
    }
}
