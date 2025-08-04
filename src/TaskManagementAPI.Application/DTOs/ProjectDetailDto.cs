namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber o conteúdo dos detalhes de um projeto.
/// </summary>
public class ProjectDetailDto
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
    /// Descrição detalhada do projeto.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Data de criação do projeto.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Lista de tarefas do projeto.
    /// </summary>
    public List<TaskProjectDto> Tasks { get; set; } = new List<TaskProjectDto>();
}