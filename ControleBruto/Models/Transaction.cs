using ControleBruto.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string UserId { get; set; }
        [Required, MaxLength(100)]
        public string Description { get; set; }
        [Required]
        public int AccountId { get; set; }
        [Required]
        public TransactionType Type { get; set; }
        public int? CategoryId { get; set; }
        [Required]
        public long AmountCents { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        // Id para agrupar as duas pontas de uma transferência
        public Guid? TransferGroupId { get; set; }

        // Navigation properties
        public User? User { get; set; }
        public Account? Account { get; set; }
        public Category? Category { get; set; }
    }
}
