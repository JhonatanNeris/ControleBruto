namespace ControleBruto.Data.Dtos.Transaction
{
    public class CreateTransferResultDto
    {
        public Guid TransferGroupId { get; set; }
        public ReadTransactionDto OutTransaction { get; set; } = null!;
        public ReadTransactionDto InTransaction { get; set; } = null!;
    }
}
