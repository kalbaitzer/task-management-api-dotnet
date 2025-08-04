using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber o conteúdo de um novo projeto a ser adicionado.
/// </summary>
public class CreateProjectDto
{
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
}