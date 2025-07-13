using TaskManagementAPI.Application.DTOs;

namespace TaskManagementAPI.Application.Interfaces;

public interface IProjectService
{
    Task<ProjectDetailDto?> GetProjectByIdAsync(Guid projectId, Guid userId);
    Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(Guid userId);
    Task<ProjectDetailDto> CreateProjectAsync(CreateProjectDto projectDto, Guid userId);
    Task DeleteProjectAsync(Guid projectId, Guid userId);
    // Poderíamos ter um método de atualização também, se necessário.
    // Task UpdateProjectAsync(Guid projectId, UpdateProjectDto projectDto, Guid userId);
}