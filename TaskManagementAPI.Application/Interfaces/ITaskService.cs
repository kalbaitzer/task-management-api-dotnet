using TaskManagementAPI.Application.DTOs;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Interfaces;

/// <summary>
/// Interface para o serviço de Tarefas.
/// </summary>
public interface ITaskService
{
    /// <summary>
    /// Cria uma nova tarefa dentro de um projeto, aplicando a regra de limite de tarefas por projeto.
    /// </summary>
    /// <param name="projectId">O ID do projeto onde a tarefa será criada.</param>
    /// <param name="createTaskDto">Os dados da nova tarefa.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Os detalhes do usuário recém-criado.</returns>
    Task<TaskDto> CreateTaskAsync(Guid projectId, CreateTaskDto taskDto, Guid userId);

    /// <summary>
    /// Lista todas as tarefas de um projeto específico.
    /// </summary>
    /// <param name="projectId">O ID do projeto.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Uma lista de tarefas do projeto.</returns>
    Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, Guid userId);

    /// <summary>
    /// Busca uma tarefa específica pelo seu ID.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Os detalhes da tarefa.</returns>
    Task<TaskDto?> GetTaskByIdAsync(Guid taskId, Guid userId);

    /// <summary>
    /// Atualiza os detalhes de uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="taskDto">Os novos dados da tarefa.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Nenhum conteúdo.</returns>
    Task UpdateTaskDetailsAsync(Guid taskId, UpdateTaskDto taskDto, Guid userId);

    /// <summary>
    /// Atualiza o status de uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="statusDto">O novo status da tarefa.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Nenhum conteúdo.</returns>
    Task UpdateTaskStatusAsync(Guid taskId, UpdateStatusDto statusDto, Guid userId);

    /// <summary>
    /// Adiciona um comentário a uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="commentDto">O comentário a ser adicionado.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Status Code 201.</returns>
    Task AddCommentAsync(Guid taskId, AddCommentDto commentDto, Guid userId);

    /// <summary>
    /// Remove uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa a ser removida.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Nenhum conteúdo.</returns>
    Task DeleteTaskAsync(Guid taskId, Guid userId);

    /// <summary>
    /// Busca o histórico completo de uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    Task<IEnumerable<TaskHistoryDto>> GetTaskHistoryAsync(Guid taskId, Guid userId);
}