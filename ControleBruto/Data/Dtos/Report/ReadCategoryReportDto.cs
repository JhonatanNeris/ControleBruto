namespace ControleBruto.Data.Dtos.Report
{
    public class ReadCategoryReportDto
    {
        public ReportPeriodDto Period { get; set; } = null!;
        public string Type { get; set; } = string.Empty;
        public IEnumerable<ReadCategoryReportItemDto> Result { get; set; } = Enumerable.Empty<ReadCategoryReportItemDto>();
    }
}
