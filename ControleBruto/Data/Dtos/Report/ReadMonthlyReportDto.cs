namespace ControleBruto.Data.Dtos.Report
{
    public class ReadMonthlyReportDto
    {
        public ReportPeriodDto Period { get; set; } = null!;
        public int TransactionsCount { get; set; }
        public long MonthlyIncome { get; set; }
        public long MonthlyExpense { get; set; }
        public long MonthlyBalance { get; set; }
        public long TotalNetWorth { get; set; }
    }
}
