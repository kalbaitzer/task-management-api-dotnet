namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// Registra uma alteração ou um comentário em uma tarefa, servindo como log de auditoria.
/// </summary>
public class TaskHistoryDto
{
    /// <summary>
    /// Identificador único do registro de histórico (Chave Primária).
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// O tipo de alteração. Ex: "Create", "Update", "Comment".
    /// Facilita a filtragem do histórico.
    /// </summary>
    public string ChangeType { get; set; } = null!;

    /// <summary>
    /// O nome do campo que foi alterado (ex: "Status", "Title").
    /// Será nulo para eventos como "Create" ou "Comment".
    /// </summary>
    public string? FieldName { get; set; }

    /// <summary>
    /// O valor antigo do campo. Nulo se não aplicável (ex: criação, comentário).
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// O novo valor do campo ou uma descrição da criação.
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// O conteúdo de um comentário. Será preenchido apenas se ChangeType for "Comment".
    /// </summary>
    public string? Comment { get; set; }

    /// <summary>
    /// A data e hora em que o registro foi criado.
    /// </summary>
    public DateTime Timestamp { get; set; }

    /// <summary>
    /// Chave Estrangeira para o Usuário que realizou a ação.
    /// </summary>
    public Guid UserId { get; set; }
}