using ControleBruto.Models;
using ControleBruto.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos.Transaction
{
    public class CreateTransactionDto
    {
        [Required, MaxLength(100)]
        public string Description { get; set; } = string.Empty;
        [Required]
        public int AccountId { get; set; }
        // Só Expense ou Income aqui (transfer vamos separar em outro DTO/endpoint)
        [Required]
        public TransactionType Type { get; set; }

        public int? CategoryId { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long AmountCents { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;
    }

}
