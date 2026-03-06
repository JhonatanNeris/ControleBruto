using ControleBruto.Data.Dtos.Report;
using ControleBruto.Models.Enums;

namespace ControleBruto.Services.Interfaces;

public interface IReportService
{
    Task<ReadMonthlyReportDto> GetMonthlyReportAsync(string userId, int month, int year);
    Task<ReadCategoryReportDto> GetReportByCategoryAsync(string userId, int month, int year, TransactionType type);

}
