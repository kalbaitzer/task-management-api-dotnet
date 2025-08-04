namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber o conteúdo do relatório de desempenho.
/// </summary>
public class PerformanceReportDto
{
    /// <summary>
    /// Nome do relatório: "Relatório de Desempenho".
    /// </summary>
    public string ReportName { get; set; } = "Relatório de Desempenho";

    /// <summary>
    /// Período do relatório: "Últimos 30 dias".
    /// </summary>
    public string Period { get; set; } = "Últimos 30 dias";

    /// <summary>
    /// Data em que o relatório foi gerado.
    /// </summary>
    public DateTime GeneratedAt { get; set; }

    /// <summary>
    /// Total de tarefas concluídas no período de 30 dias.
    /// </summary>
    public int TotalTasksCompleted { get; set; }

    /// <summary>
    /// Número de usuários distintos que concluiram tarefas no período de 30 dias.
    /// </summary>
    public int DistinctUsersWhoCompletedTasks { get; set; }

    /// <summary>
    /// Média de tarefas concluídas por usuário no período de 30 dias.
    /// </summary>
    public double AverageTasksCompletedPerUser { get; set; }
}