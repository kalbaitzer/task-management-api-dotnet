using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Core.Entities;

/// <summary>
/// Registra uma alteração ou um comentário em uma tarefa, servindo como log de auditoria.
/// </summary>
public class TaskHistory
{
    /// <summary>
    /// Identificador único do registro de histórico (Chave Primária).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; } = Guid.NewGuid();

    /// <summary>
    /// O tipo de alteração. Ex: "Create", "Update", "Comment".
    /// Facilita a filtragem do histórico.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string ChangeType { get; private set; }

    /// <summary>
    /// O nome do campo que foi alterado (ex: "Status", "Title").
    /// Será nulo para eventos como "Create" ou "Comment".
    /// </summary>
    [MaxLength(100)]
    public string? FieldName { get; private set; }

    /// <summary>
    /// O valor antigo do campo. Nulo se não aplicável (ex: criação, comentário).
    /// </summary>
    public string? OldValue { get; private set; }

    /// <summary>
    /// O novo valor do campo ou uma descrição da criação.
    /// </summary>
    public string? NewValue { get; private set; }

    /// <summary>
    /// O conteúdo de um comentário. Será preenchido apenas se ChangeType for "Comment".
    /// </summary>
    public string? Comment { get; private set; }

    /// <summary>
    /// A data e hora em que o registro foi criado.
    /// </summary>
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    // --- Relacionamentos ---

    /// <summary>
    /// Chave Estrangeira para a Tarefa à qual este histórico pertence.
    /// </summary>
    public Guid TaskId { get; private set; }

    /// <summary>
    /// Propriedade de Navegação para a Tarefa.
    /// </summary>
    [ForeignKey("TaskId")]
    public Task Task { get; private set; } = null!;

    /// <summary>
    /// Chave Estrangeira para o Usuário que realizou a ação.
    /// </summary>
    public Guid UserId { get; private set; }

    /// <summary>
    /// Propriedade de Navegação para o Usuário.
    /// </summary>
    [ForeignKey("UserId")]
    public User User { get; private set; } = null!;

    /// <summary>
    /// Construtor privado para o EF Core.
    /// </summary>
    private TaskHistory()
    {
        // Necessário para o EF Core
        ChangeType = string.Empty;
    }

    /// <summary>
    /// Construtor ppúblico.
    /// </summary>
    public TaskHistory(string changeType, string fieldName, string oldValue, string newValue, string comment, Guid taskId, Guid userId)
    {
        ChangeType = changeType;
        FieldName = fieldName;
        OldValue = oldValue;
        NewValue = newValue;
        Comment = comment;
        TaskId = taskId;
        UserId = userId;
    }

    // Construtores estáticos (Static Factory Methods) para criar registros de forma clara e segura

        /// <summary>
        /// Cria um registro de histórico para uma atualização de campo.
        /// </summary>
    public static TaskHistory ForUpdate(Guid taskId, Guid userId, string fieldName, string? oldValue, string? newValue)
    {
        return new TaskHistory
        {
            ChangeType = "Update",
            FieldName = fieldName,
            OldValue = oldValue,
            NewValue = newValue,
            TaskId = taskId,
            UserId = userId
        };
    }

    /// <summary>
    /// Cria um registro de histórico para um novo comentário.
    /// </summary>
    public static TaskHistory ForComment(Guid taskId, Guid userId, string comment)
    {
        return new TaskHistory
        {
            ChangeType = "Comment",
            Comment = comment,
            TaskId = taskId,
            UserId = userId
        };
    }

    /// <summary>
    /// Cria um registro de histórico para a criação da tarefa.
    /// </summary>
    public static TaskHistory ForCreation(Guid taskId, Guid userId, string title)
    {
        return new TaskHistory
        {
            ChangeType = "Create",
            NewValue = $"Tarefa '{title}' foi criada.",
            TaskId = taskId,
            UserId = userId
        };
    }
}