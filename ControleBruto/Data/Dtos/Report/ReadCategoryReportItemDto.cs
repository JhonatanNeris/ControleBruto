namespace ControleBruto.Data.Dtos.Report
{
    public class ReadCategoryReportItemDto
    {
        public int? CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public long TotalCents { get; set; }
        public int Count { get; set; }
    }
}
