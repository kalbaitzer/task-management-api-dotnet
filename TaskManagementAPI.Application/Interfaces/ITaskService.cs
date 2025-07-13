using TaskManagementAPI.Application.DTOs;

namespace TaskManagementAPI.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(Guid projectId, CreateTaskDto taskDto, Guid userId);
    Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, Guid userId);
    Task<TaskDto?> GetTaskByIdAsync(Guid taskId, Guid userId);
    System.Threading.Tasks.Task UpdateTaskAsync(Guid taskId, UpdateTaskDto taskDto, Guid userId);
    System.Threading.Tasks.Task UpdateTaskStatusAsync(Guid taskId, UpdateStatusDto statusDto, Guid userId);
    System.Threading.Tasks.Task AddCommentAsync(Guid taskId, AddCommentDto commentDto, Guid userId);
    System.Threading.Tasks.Task DeleteTaskAsync(Guid taskId, Guid userId);
    Task<IEnumerable<TaskHistoryDto>> GetTaskHistoryAsync(Guid taskId, Guid userId);
}