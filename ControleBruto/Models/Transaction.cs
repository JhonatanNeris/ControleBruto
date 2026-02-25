namespace ControleBruto.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public int AccountId { get; set; }
        public int CategoryId { get; set; }
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string Description { get; set; }
        // Navigation properties
        public Account Account { get; set; }
        public Category Category { get; set; }
    }
}
