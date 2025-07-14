using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;
using TaskManagementAPI.Infrastructure.Data;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório de Histórico de Tarefas, usando Entity Framework Core.
/// </summary>
public class TaskHistoryRepository : ITaskHistoryRepository
{
    private readonly ApplicationDbContext _context;

    public TaskHistoryRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo registro de histórico ao contexto do banco de dados.
    /// A persistência ocorre quando a Unidade de Trabalho salva as alterações.
    /// </summary>
    /// <param name="history">A entidade de histórico a ser adicionada.</param>
    public async Task AddAsync(TaskHistory history)
    {
        await _context.TaskHistories.AddAsync(history);
    }

    /// <summary>
    /// Busca todo o histórico de uma tarefa específica.
    /// </summary>
    /// <param name="taskId">O ID da tarefa cujo histórico será recuperado.</param>
    /// <returns>Uma coleção de registros de histórico.</returns>
    public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId)
    {
        // Include(th => th.User) é usado para carregar os dados do usuário que realizou a ação.
        // Isso é essencial para que a API possa retornar "Quem" fez a alteração.
        // OrderByDescending(th => th.Timestamp) garante que os eventos mais recentes apareçam primeiro.
        return await _context.TaskHistories
            .Include(th => th.User)
            .Where(th => th.TaskId == taskId)
            .OrderByDescending(th => th.Timestamp)
            .ToListAsync();
    }
}