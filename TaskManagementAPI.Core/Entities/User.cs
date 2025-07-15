using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace TaskManagementAPI.Core.Entities;

/// <summary>
/// Representa um Usuário do sistema.
/// </summary>
public class User
{
    /// <summary>
    /// Identificador único do usuário (Chave Primária).
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Nome completo do usuário.
    /// </summary>
    [Required]
    [MaxLength(150)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Endereço de e-mail do usuário, usado para login. Deve ser único.
    /// A unicidade será configurada no DbContext.
    /// </summary>
    [Required]
    [EmailAddress]
    [MaxLength(150)]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Papel (Role) do usuário no sistema, para controle de permissões.
    /// Ex: "User", "Manager". Utilizado na Regra de Negócio 5.
    /// </summary>
    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = string.Empty;
 
    // A coleção de projetos que este usuário possui.
    public ICollection<Project> Projects { get; set; } = new List<Project>();

    // A coleção de histórico de tarefas que foram alteradas por este usuário.
    public ICollection<TaskHistory> TaskHistories { get; set; } = new List<TaskHistory>();
}