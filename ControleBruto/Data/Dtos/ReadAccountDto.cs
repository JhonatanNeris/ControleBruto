namespace ControleBruto.Data.Dtos
{
    public class ReadAccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public long InitialBalanceCents { get; set; }
    }
}
