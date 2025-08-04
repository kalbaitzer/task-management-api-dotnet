using TaskManagementAPI.Application.DTOs;

namespace TaskManagementAPI.Application.Interfaces;

/// <summary>
/// Interface para o serviço de Relatórios.
/// </summary>
public interface IReportService
{
    /// <summary>
    /// Gera um relatório de desempenho com a média de tarefas concluídas por usuário nos últimos 30 dias.
    /// Esta funcionalidade implementa a Regra de Negócio 5.
    /// </summary>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Um DTO com os dados do relatório.</returns>
    Task<PerformanceReportDto> GenerateAverageCompletedTasksReportAsync(Guid userId);
}