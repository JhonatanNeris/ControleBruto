using ControleBruto.Data.Dtos.Category;
using ControleBruto.Models.Enums;

namespace ControleBruto.Data.Dtos.Transaction
{
    public class ReadTransactionDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int AccountId { get; set; }
        public TransactionType Type { get; set; }
        public ReadCategoryDto? Category { get; set; }
        public long AmountCents { get; set; }
        public DateTime Date { get; set; }
        public Guid? TransferGroupId { get; set; }
    }
}
