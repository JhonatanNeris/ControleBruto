using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Models
{
    public class Account
    {
        [Key]
        [Required]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required(ErrorMessage = "O nome da conta é obrigatório.")]
        [MaxLength(100)]
        public string Name { get; set; }
        [Required]
        [MaxLength(100)]
        public string Type { get; set; }
        public long InitialBalanceCents { get; set; } = 0;
        public User? User { get; set; }
        public ICollection<Transaction>? Transactions { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, UserId: {UserId}";
        }
    }
} 
