namespace ControleBruto.Data.Dtos
{
    public class TokenDto
    {
        public string AccessToken { get; set; }
        public DateTime ExpiresAtUtc { get; set; }
    }
}
