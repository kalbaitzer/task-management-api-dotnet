using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber o conteúdo de uma nova tarefa a ser adicionada.
/// </summary>
public class CreateTaskDto
{
    /// <summary>
    /// Título da tarefa. Campo obrigatório com no máximo 200 caracteres.
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Descrição detalhada da tarefa. Opcional, com no máximo 1000 caracteres.
    /// </summary>
    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data de vencimento da tarefa.
    /// </summary>
    [Required]
    public DateTime DueDate { get; set; }

    /// <summary>
    /// Prioridade da tarefa (Baixa, Média, Alta).
    /// O 'private set' implementa a Regra de Negócio 1: a prioridade não pode ser
    /// alterada após a criação, exceto por métodos dentro desta classe.
    /// </summary>
    [Required]
    public TaskPriority Priority { get; set; }
}