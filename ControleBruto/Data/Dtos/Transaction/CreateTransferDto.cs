using ControleBruto.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace ControleBruto.Data.Dtos.Transaction
{
    public class CreateTransferDto
    {
        [Required, MaxLength(100)]
        public string Description { get; set; } = "Transferência";

        [Required]
        public int FromAccountId { get; set; }

        [Required]
        public int ToAccountId { get; set; }

        [Required]
        [Range(1, long.MaxValue)]
        public long AmountCents { get; set; }

        [Required]
        public DateTime Date { get; set; }
    }
}
