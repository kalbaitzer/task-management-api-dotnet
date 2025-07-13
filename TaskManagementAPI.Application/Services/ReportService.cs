using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.Application.Services;

public class ReportService : IReportService
{
    private readonly ITaskRepository _taskRepository;

    public ReportService(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    /// <summary>
    /// Gera um relatório de desempenho com a média de tarefas concluídas por usuário nos últimos 30 dias.
    /// Esta funcionalidade implementa a Regra de Negócio 5.
    /// </summary>
    /// <returns>Um DTO com os dados do relatório.</returns>
    public async Task<PerformanceReportDto> GenerateAverageCompletedTasksReportAsync()
    {
        // Definir o período de análise (últimos 30 dias)
        var startDate = DateTime.UtcNow.AddDays(-30);

        // Delegar a busca dos dados brutos ao repositório.
        // O repositório deve ter um método otimizado para esta consulta.
        var completedTasks = await _taskRepository.GetCompletedTasksSinceAsync(startDate);

        // Processar os dados para gerar as métricas
        int totalTasksCompleted = completedTasks.Count();

        // Lidar com o caso de não haver tarefas para evitar divisão por zero
        if (totalTasksCompleted == 0)
        {
            return new PerformanceReportDto
            {
                GeneratedAt = DateTime.UtcNow,
                TotalTasksCompleted = 0,
                ActiveUsersInPeriod = 0,
                AverageTasksCompletedPerUser = 0
            };
        }

        // Calcular o número de usuários distintos que completaram as tarefas
        int activeUsersInPeriod = completedTasks
            .Select(task => task.UserId) // Usamos o UserId para contabilizar os usuários distintos que concluiram tarefas
            .Distinct()
            .Count();

        // Calcular a média
        double averageTasks = (double)totalTasksCompleted / activeUsersInPeriod;

        // Montar e retornar o DTO do relatório
        return new PerformanceReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            TotalTasksCompleted = totalTasksCompleted,
            ActiveUsersInPeriod = activeUsersInPeriod,
            AverageTasksCompletedPerUser = Math.Round(averageTasks, 2) // Arredonda para 2 casas decimais
        };
    }
}