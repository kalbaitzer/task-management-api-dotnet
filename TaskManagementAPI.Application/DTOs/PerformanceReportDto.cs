namespace TaskManagementAPI.Application.DTOs;

public class PerformanceReportDto
{
    public string ReportName { get; set; } = "Relatório de Desempenho";
    public string Period { get; set; } = "Últimos 30 dias";
    public DateTime GeneratedAt { get; set; }
    public int TotalTasksCompleted { get; set; }
    public int DistinctUsersWhoCompletedTasks { get; set; }
    public double AverageTasksCompletedPerUser { get; set; }
}