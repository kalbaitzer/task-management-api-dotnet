namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber o um resumo do projeto.
/// </summary>
public class ProjectDto
{
    /// <summary>
    /// Identificador único do projeto.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do projeto.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Número de tarefas do projeto.
    /// </summary>
    public int TaskCount { get; set; }
}