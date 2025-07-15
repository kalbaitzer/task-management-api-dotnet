using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de Histórico de Tarefas.
/// Define o contrato para as operações de dados relacionadas à auditoria de tarefas.
/// O design desta interface é primariamente focado na adição de novos registros
/// e na leitura do histórico completo de uma tarefa.
/// </summary>
public interface ITaskHistoryRepository
{
    /// <summary>
    /// Adiciona um novo registro de histórico ao contexto do banco de dados.
    /// Este é o método principal usado para registrar qualquer evento
    /// (criação, atualização, comentário) em uma tarefa.
    /// </summary>
    /// <param name="history">A entidade de histórico a ser adicionada.</param>
    Task AddAsync(TaskHistory history);

    /// <summary>
    /// Busca todo o histórico de alterações e comentários de uma tarefa específica,
    /// ordenado do mais recente para o mais antigo.
    /// Usado para exibir a trilha de auditoria na interface do usuário.
    /// </summary>
    /// <param name="taskId">O ID da tarefa para a qual o histórico será recuperado.</param>
    /// <returns>Uma coleção de registros de histórico da tarefa.</returns>
    Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(Guid taskId);
}