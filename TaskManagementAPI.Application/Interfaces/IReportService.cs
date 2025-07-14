using TaskManagementAPI.Application.DTOs;

namespace TaskManagementAPI.Application.Interfaces;

public interface IReportService
{
    Task<PerformanceReportDto> GenerateAverageCompletedTasksReportAsync(Guid userId);
}