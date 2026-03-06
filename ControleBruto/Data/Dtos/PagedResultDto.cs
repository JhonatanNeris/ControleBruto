namespace ControleBruto.Data.Dtos
{
    public class PagedResultDto<T>
    {
        public int Total { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    }
}
