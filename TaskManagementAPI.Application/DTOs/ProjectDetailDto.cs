namespace TaskManagementAPI.Application.DTOs;

public class ProjectDetailDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public List<TaskProjectDto> Tasks { get; set; } = new List<TaskProjectDto>();
}