using ControleBruto.Models.Enums;

namespace ControleBruto.Data.Dtos.Transaction
{
    public class TransactionQueryDto
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int? AccountId { get; set; }
        public int? CategoryId { get; set; }
        public TransactionType? Type { get; set; }
        public string? Search { get; set; }
        public int Skip { get; set; } = 0;
        public int Take { get; set; } = 10;
    }
}
