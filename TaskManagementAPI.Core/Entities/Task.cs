using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Core.Entities;

/// <summary>
/// Define os status possíveis para uma tarefa. Usar um enum é uma forma de validação,
/// pois restringe os valores a um conjunto pré-definido.
/// </summary>
public enum TaskStatus { Pendente, EmAndamento, Concluida }

/// <summary>
/// Define as prioridades possíveis para uma tarefa.
/// </summary>
public enum TaskPriority { Baixa, Media, Alta }

/// <summary>
/// Representa uma unidade de trabalho dentro de um projeto.
/// </summary>
public class Task
{
    /// <summary>
    /// Identificador único da tarefa (Chave Primária).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; private set; }

    /// <summary>
    /// Título da tarefa. Campo obrigatório com no máximo 200 caracteres.
    /// </summary>
    [Required(ErrorMessage = "O título da tarefa é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O título não pode exceder 200 caracteres.")]
    public string Title { get; private set; }

    /// <summary>
    /// Descrição detalhada da tarefa. Opcional, com no máximo 1000 caracteres.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
    public string Description { get; private set; }

    /// <summary>
    /// Data de vencimento da tarefa.
    /// </summary>
    public DateTime DueDate { get; private set; }

    /// <summary>
    /// Status atual da tarefa (Pendente, Em Andamento, Concluída).
    /// </summary>
    public TaskStatus Status { get; private set; }

    /// <summary>
    /// Prioridade da tarefa (Baixa, Média, Alta).
    /// O 'private set' implementa a Regra de Negócio 1: a prioridade não pode ser
    /// alterada após a criação, exceto por métodos dentro desta classe.
    /// </summary>
    public TaskPriority Priority { get; private set; }

    /// <summary>
    /// Data de criação da tarefa.
    /// </summary>
    public DateTime CreatedAt { get; private set; }

    /// <summary>
    /// Data de atualização da tarefa.
    /// </summary>
    public DateTime UpdatedAt { get; set; }

    // --- Relacionamentos ---

    /// <summary>
    /// Chave Estrangeira para a entidade Project.
    /// </summary>
    public Guid ProjectId { get; private set; }

    /// <summary>
    /// Propriedade de Navegação para o projeto ao qual a tarefa pertence.
    /// </summary>
    [ForeignKey("ProjectId")]
    public Project Project { get; private set; } = null!;

    /// <summary>
    /// Coleção de registros de histórico para esta tarefa (Regra de Negócio 3).
    /// </summary>
    public ICollection<TaskHistory> TaskHistories { get; private set; } = new List<TaskHistory>();

    /// <summary>
    /// Construtor privado exigido pelo Entity Framework Core para materialização de objetos.
    /// </summary>
    private Task()
    {
        // Necessário para o EF Core
        Title = string.Empty;
        Description = string.Empty;
    }

    /// <summary>
    /// Construtor público para criar uma nova tarefa de forma controlada e válida.
    /// </summary>
    /// <param name="title">Título da tarefa.</param>
    /// <param name="description">Descrição detalhada da tarefa.</param>
    /// <param name="dueDate">Data de vencimento da tarefa.</param>
    /// <param name="priority">Prioridade da tarefa (Baixa, Média, Alta).</param>
    /// <param name="projectId">Chave Estrangeira para a entidade Project.</param>
    public Task(string title, string description, DateTime dueDate, TaskPriority priority, Guid projectId)
    {
        Id = Guid.NewGuid();
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority; // Prioridade é definida aqui e não pode ser alterada externamente.
        ProjectId = projectId;

        Status = TaskStatus.Pendente; // Status inicial padrão
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Atualiza os detalhes da tarefa.
    /// </summary>
    /// <param name="newtitle">Novo título da tarefa.</param>
    /// <param name="newDescription">Nova descrição da tarefa.</param>
    /// <param name="newDueDate">Nova data de vencimento da tarefa.</param>
    /// <param name="newStatus">Novo status da tarefa.</param>
    /// <param name="newUpdatedAt">Nova data de atualização da tarefa.</param>
    /// <returns>Nenhum conteúdo.</returns>
    public void UpdateDetails(string newTitle, string newDescription, DateTime newDueDate, TaskStatus newStatus, DateTime newUpdateAt)
    {
        Title = newTitle;
        Description = newDescription;
        DueDate = newDueDate;
        Status = newStatus;
        UpdatedAt = newUpdateAt;
    }

    /// <summary>
    /// Atualiza o status da tarefa.
    /// </summary>
    /// <param name="newStatus">Novo status da tarefa.</param>
    /// <param name="newUpdatedAt">Nova data de atualização da tarefa.</param>
    /// <returns>Nenhum conteúdo.</returns>
    // Métodos públicos para alterar o estado da tarefa de forma controlada
    public void UpdateStatus(TaskStatus newStatus, DateTime newUpdateAt)
    {
        // Verificação do Status atual
        if (Status == TaskStatus.Concluida && newStatus != TaskStatus.Concluida)
        {
            throw new InvalidOperationException("Não é possível reabrir uma tarefa concluída.");
        }

        Status = newStatus;
        UpdatedAt = newUpdateAt;
    }
}