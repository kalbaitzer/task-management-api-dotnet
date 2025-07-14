using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services.Utils;

namespace TaskManagementAPI.Application.Services;

public class ReportService : IReportService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;

    public ReportService(ITaskRepository taskRepository, IUserRepository userRepository)
    {
        _taskRepository = taskRepository;
        _userRepository = userRepository;
    }

    /// <summary>
    /// Gera um relatório de desempenho com a média de tarefas concluídas por usuário nos últimos 30 dias.
    /// Esta funcionalidade implementa a Regra de Negócio 5.
    /// </summary>
    /// <returns>Um DTO com os dados do relatório.</returns>
    public async Task<PerformanceReportDto> GenerateAverageCompletedTasksReportAsync(Guid userId)
    {
        // Verifica se o usuário existe
        var user = await ServiceHelper.CheckUser(userId, _userRepository);

        if (user != null && user.Role != "Manager")
        {
            // Lança uma exceção de regra de negócio
            throw new BusinessRuleException("Você não tem permissão para acessar este relatório.");
        }

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
                DistinctUsersWhoCompletedTasks = 0,
                AverageTasksCompletedPerUser = 0
            };
        }

        // Calcular o número de usuários distintos que completaram as tarefas
        int distinctUsersWhoCompletedTasks = completedTasks
            .Select(task => task.UserId) // Usamos o UserId para contabilizar os usuários distintos que concluiram tarefas
            .Distinct()
            .Count();

        // Calcular a média
        double averageTasks = (double)totalTasksCompleted / distinctUsersWhoCompletedTasks;

        // Montar e retornar o DTO do relatório
        return new PerformanceReportDto
        {
            GeneratedAt = DateTime.UtcNow,
            TotalTasksCompleted = totalTasksCompleted,
            DistinctUsersWhoCompletedTasks = distinctUsersWhoCompletedTasks,
            AverageTasksCompletedPerUser = Math.Round(averageTasks, 2) // Arredonda para 2 casas decimais
        };
    }
}