using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;
using TaskEntity = TaskManagementAPI.Core.Entities.Task;

namespace TaskManagementAPI.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de Tarefas. Define o contrato para as operações de dados
/// relacionadas a tarefas, abstraindo a camada de acesso a dados.
/// </summary>
public interface ITaskRepository
{
    /// <summary>
    /// Adiciona uma nova tarefa ao contexto do banco de dados.
    /// Usado pelo TaskService no processo de criação.
    /// </summary>
    /// <param name="task">A entidade da tarefa a ser adicionada.</param>
    Task AddAsync(TaskEntity task);

    /// <summary>
    /// Busca uma única tarefa pelo seu ID.
    /// Usado para atualizações, deleções e visualização de detalhes.
    /// </summary>
    /// <param name="id">O ID da tarefa.</param>
    /// <returns>A entidade da tarefa ou nulo se não for encontrada.</returns>
    Task<TaskEntity?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lista todas as tarefas de um projeto específico.
    /// Usado pelo TaskService para exibir as tarefas de um projeto.
    /// </summary>
    /// <param name="projectId">O ID do projeto.</param>
    /// <returns>Uma coleção de tarefas.</returns>
    Task<IEnumerable<TaskEntity>> GetByProjectIdAsync(Guid projectId);

    /// <summary>
    /// Marca uma tarefa para remoção do banco de dados.
    /// A operação de salvar (commit) é feita pela Unidade de Trabalho (Unit of Work).
    /// </summary>
    /// <param name="task">A entidade da tarefa a ser removida.</param>
    void Delete(TaskEntity task);

    /// <summary>
    /// Verifica se um projeto contém tarefas ativas (Pendente ou Em Andamento).
    /// Este método é específico para suportar a Regra de Negócio 2.
    /// Usado pelo ProjectService antes de tentar remover um projeto.
    /// </summary>
    /// <param name="projectId">O ID do projeto a ser verificado.</param>
    /// <returns>Verdadeiro se houver tarefas ativas, senão falso.</returns>
    Task<bool> HasActiveTasksInProjectAsync(Guid projectId);

    /// <summary>
    /// Busca todas as tarefas concluídas a partir de uma data específica.
    /// Este método é específico para suportar a Regra de Negócio 5.
    /// Usado pelo ReportService para gerar o relatório de desempenho.
    /// </summary>
    /// <param name="startDate">A data inicial do período de busca.</param>
    /// <returns>Uma coleção de tarefas concluídas.</returns>
    Task<IEnumerable<TaskHistory>> GetCompletedTasksSinceAsync(DateTime startDate);
}