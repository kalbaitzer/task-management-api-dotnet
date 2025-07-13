using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TaskManagementAPI.Core.Entities;

/// <summary>
/// Representa um Projeto, que é um contêiner para um conjunto de tarefas.
/// </summary>
public class Project
{
    /// <summary>
    /// Identificador único do projeto (Chave Primária).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome do projeto. É um campo obrigatório.
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada do projeto.
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do projeto, definida automaticamente.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // --- Relacionamentos ---

    /// <summary>
    /// Chave Estrangeira (Foreign Key) para a entidade User.
    /// Indica quem é o proprietário do projeto.
    /// </summary>
    public Guid OwnerUserId { get; set; }

    /// <summary>
    /// Propriedade de Navegação para o usuário, o qual criou o projeto
    /// </summary>
    [ForeignKey("OwnerUserId")]
    public User Owner { get; set; } = null!; // O 'null!' diz ao compilador para não se preocupar, o EF Core irá preenchê-la.

    /// <summary>
    /// Propriedade de Navegação para a coleção de Tarefas associadas a este projeto.
    /// O relacionamento é de um Projeto para muitas Tarefas.
    /// Inicializar a coleção previne erros de referência nula (NullReferenceException).
    /// </summary>
    public ICollection<Task> Tasks { get; set; } = new List<Task>();
}