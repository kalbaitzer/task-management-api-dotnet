using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;
using TaskManagementAPI.Infrastructure.Data;

// Usando o alias para a entidade Task
using Task = System.Threading.Tasks.Task;
using TaskEntity = TaskManagementAPI.Core.Entities.Task;

namespace TaskManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório de Tarefas, usando Entity Framework Core.
/// </summary>
public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// O construtor recebe o DbContext via injeção de dependência.
    /// </summary>
    /// <param name="context">O DbContext da aplicação.</param>
    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona uma nova tarefa ao contexto. A persistência ocorre ao chamar SaveChangesAsync.
    /// </summary>
    public async Task AddAsync(TaskEntity task)
    {
        await _context.Tasks.AddAsync(task);
    }

    /// <summary>
    /// Marca uma tarefa para deleção. A remoção do banco ocorre ao chamar SaveChangesAsync.
    /// </summary>
    public void Delete(TaskEntity task)
    {
        _context.Tasks.Remove(task);
    }

    /// <summary>
    /// Busca uma tarefa pelo seu ID de forma otimizada.
    /// </summary>
    public async Task<TaskEntity?> GetByIdAsync(Guid id)
    {
        // FindAsync é otimizado para buscar por chave primária.
        return await _context.Tasks.FindAsync(id);
    }

    /// <summary>
    /// Busca todas as tarefas de um projeto, ordenadas por data de criação.
    /// </summary>
    public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Implementação da Regra de Negócio 5: Busca tarefas concluídas desde uma data.
    /// </summary>
    public async Task<IEnumerable<TaskHistory>> GetCompletedTasksSinceAsync(DateTime startDate)
    {
        // Obtém no histórico de alteração de tarefas, todas as que foram concluídas após a data startDate.
        return await _context.TaskHistories
            .Where(t => t.ChangeType == "Update" && t.NewValue == "Concluida" && t.Timestamp >= startDate)
            .ToListAsync();
    }

    /// <summary>
    /// Implementação da Regra de Negócio 2: Verifica se existem tarefas ativas em um projeto.
    /// </summary>
    public async Task<bool> HasActiveTasksInProjectAsync(Guid projectId)
    {
        // AnyAsync é extremamente eficiente para verificações de existência.
        // Ele se traduz em uma consulta "EXISTS" no SQL, parando assim que encontra o primeiro registro.
        return await _context.Tasks.AnyAsync(t =>
            t.ProjectId == projectId &&
            (t.Status == TaskManagementAPI.Core.Entities.TaskStatus.Pendente || t.Status == TaskManagementAPI.Core.Entities.TaskStatus.EmAndamento));
    }
}