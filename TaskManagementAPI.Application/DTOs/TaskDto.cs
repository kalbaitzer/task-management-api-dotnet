using TaskManagementAPI.Core.Entities; // Para usar os enums TaskStatus e TaskPriority

namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// Representa os dados de uma Tarefa que são enviados para o cliente da API.
/// Este DTO é um "contrato" público e seguro, expondo apenas os dados necessários.
/// </summary>
public class TaskDto
{
    /// <summary>
    /// O ID único da tarefa.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID do projeto.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// O título da tarefa.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A descrição detalhada da tarefa.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// A data de vencimento da tarefa.
    /// </summary>
    public DateTime DueDate { get; set; }

    /// <summary>
    /// O status atual da tarefa. Usar o enum diretamente torna a API fortemente tipada.
    /// O cliente saberá que os valores possíveis são 0 (Pendente), 1 (EmAndamento), 2 (Concluida).
    /// </summary>
    public TaskManagementAPI.Core.Entities.TaskStatus Status { get; set; }

    /// <summary>
    /// A prioridade da tarefa.
    /// </summary>
    public TaskPriority Priority { get; set; }

    /// <summary>
    /// A data em que a tarefa foi criada.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Data de atualização da tarefa.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}