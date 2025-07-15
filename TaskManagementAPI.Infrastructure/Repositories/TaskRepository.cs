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
    /// Construtor do repositório de Projetos.
    /// </summary>
    /// <param name="context">Núcleo do Entity FRamework Core que interage com o banco de dados.</param>
    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona uma nova tarefa ao contexto do banco de dados.
    /// Usado pelo TaskService no processo de criação.
    /// </summary>
    /// <param name="task">A entidade da tarefa a ser adicionada.</param>
    public async Task AddAsync(TaskEntity task)
    {
        await _context.Tasks.AddAsync(task);
    }

    /// <summary>
    /// Busca uma única tarefa pelo seu ID.
    /// Usado para atualizações, deleções e visualização de detalhes.
    /// </summary>
    /// <param name="id">O ID da tarefa.</param>
    /// <returns>A entidade da tarefa ou nulo se não for encontrada.</returns>
    public async Task<TaskEntity?> GetByIdAsync(Guid id)
    {
        // FindAsync é otimizado para buscar por chave primária.
        return await _context.Tasks.FindAsync(id);
    }

    /// <summary>
    /// Lista todas as tarefas de um projeto específico.
    /// Usado pelo TaskService para exibir as tarefas de um projeto.
    /// </summary>
    /// <param name="projectId">O ID do projeto.</param>
    /// <returns>Uma coleção de tarefas.</returns>
    public async Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId)
    {
        return await _context.Tasks
            .Where(t => t.ProjectId == projectId)
            .OrderBy(t => t.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Marca uma tarefa para remoção do banco de dados.
    /// A operação de salvar (commit) é feita pela Unidade de Trabalho (Unit of Work).
    /// </summary>
    /// <param name="task">A entidade da tarefa a ser removida.</param>
    /// <returns>Nenhum conteúdo.</returns>
    public void Delete(TaskEntity task)
    {
        _context.Tasks.Remove(task);
    }

    /// <summary>
    /// Verifica se um projeto contém tarefas ativas (Pendente ou Em Andamento).
    /// Este método é específico para suportar a Regra de Negócio 2.
    /// Usado pelo ProjectService antes de tentar remover um projeto.
    /// </summary>
    /// <param name="projectId">O ID do projeto a ser verificado.</param>
    /// <returns>Verdadeiro se houver tarefas ativas, senão falso.</returns>
    public async Task<bool> HasActiveTasksInProjectAsync(Guid projectId)
    {
        // AnyAsync é extremamente eficiente para verificações de existência.
        // Ele se traduz em uma consulta "EXISTS" no SQL, parando assim que encontra o primeiro registro.
        return await _context.Tasks.AnyAsync(t =>
            t.ProjectId == projectId &&
            (t.Status == TaskManagementAPI.Core.Entities.TaskStatus.Pendente || t.Status == TaskManagementAPI.Core.Entities.TaskStatus.EmAndamento));
    }

    /// <summary>
    /// Busca todas as tarefas concluídas a partir de uma data específica.
    /// Este método é específico para suportar a Regra de Negócio 5.
    /// Usado pelo ReportService para gerar o relatório de desempenho.
    /// </summary>
    /// <param name="startDate">A data inicial do período de busca.</param>
    /// <returns>Uma coleção de tarefas concluídas.</returns>
    public async Task<IEnumerable<TaskHistory>> GetCompletedTasksSinceAsync(DateTime startDate)
    {
        // Obtém no histórico de alteração de tarefas, todas as que foram concluídas após a data startDate.
        return await _context.TaskHistories
            .Where(t => t.ChangeType == "Update" && t.NewValue == "Concluida" && t.Timestamp >= startDate)
            .ToListAsync();
    }
}