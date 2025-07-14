using TaskManagementAPI.Application.DTOs;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Interfaces;

public interface ITaskService
{
    Task<TaskDto> CreateTaskAsync(Guid projectId, CreateTaskDto taskDto, Guid userId);
    Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, Guid userId);
    Task<TaskDto?> GetTaskByIdAsync(Guid taskId, Guid userId);
    Task UpdateTaskDetailsAsync(Guid taskId, UpdateTaskDto taskDto, Guid userId);
    Task UpdateTaskStatusAsync(Guid taskId, UpdateStatusDto statusDto, Guid userId);
    Task AddCommentAsync(Guid taskId, AddCommentDto commentDto, Guid userId);
    Task DeleteTaskAsync(Guid taskId, Guid userId);
    Task<IEnumerable<TaskHistoryDto>> GetTaskHistoryAsync(Guid taskId, Guid userId);
}