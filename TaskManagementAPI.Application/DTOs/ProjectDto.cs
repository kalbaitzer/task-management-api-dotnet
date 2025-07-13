namespace TaskManagementAPI.Application.DTOs;

public class ProjectDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TaskCount { get; set; }
}