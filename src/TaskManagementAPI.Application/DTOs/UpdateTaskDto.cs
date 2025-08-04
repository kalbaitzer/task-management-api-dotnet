using System.ComponentModel.DataAnnotations;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using TaskStatus = TaskManagementAPI.Core.Entities.TaskStatus;

namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO utilizado para receber os dados de atualização de uma tarefa.
/// Note a ausência da propriedade 'Priority', que é uma decisão de design
/// para garantir a Regra de Negócio de que a prioridade não pode ser alterada.
/// </summary>
public class UpdateTaskDto
{
    /// <summary>
    /// O novo título da tarefa.
    /// </summary>
    [Required(ErrorMessage = "O título é obrigatório.")]
    [MaxLength(200, ErrorMessage = "O título não pode exceder 200 caracteres.")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// A nova descrição detalhada da tarefa.
    /// </summary>
    [MaxLength(1000, ErrorMessage = "A descrição não pode exceder 1000 caracteres.")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// A nova data de vencimento da tarefa.
    /// </summary>
    [Required(ErrorMessage = "A data de vencimento é obrigatória.")]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// O novo status da tarefa.
    /// </summary>
    [Required(ErrorMessage = "O status é obrigatório.")]
    public TaskStatus Status { get; set; }
}