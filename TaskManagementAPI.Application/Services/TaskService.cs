using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services.Utils;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;
using TaskEntity = TaskManagementAPI.Core.Entities.Task;

namespace TaskManagementAPI.Application.Services;

public class TaskService : ITaskService
{
    private readonly ITaskRepository _taskRepository;
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskHistoryRepository _taskHistoryRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    // Injeção de Dependência dos repositórios e da Unidade de Trabalho
    public TaskService(
        ITaskRepository taskRepository,
        IProjectRepository projectRepository,
        ITaskHistoryRepository taskHistoryRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _taskRepository = taskRepository;
        _projectRepository = projectRepository;
        _taskHistoryRepository = taskHistoryRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria uma nova tarefa, aplicando as regras de negócio.
    /// </summary>
    public async Task<TaskDto> CreateTaskAsync(Guid projectId, CreateTaskDto taskDto, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verfica se o projeto existe
        var project = await ServiceHelper.CheckProject(projectId, _projectRepository);

        // Regra de Negócio 4: Limite de tarefas por projeto
        if (project != null && project.Tasks.Count >= 20)
        {
            // Lança uma exceção de regra de negócio
            throw new BusinessRuleException("Limite de 20 tarefas por projeto foi atingido.");
        }

        // Cria a entidade Tarefa de forma controlada através do seu construtor
        var task = new TaskEntity(taskDto.Title, taskDto.Description, taskDto.DueDate, taskDto.Priority, projectId);

        // Adiciona a tarefa ao repositório
        await _taskRepository.AddAsync(task);

        // Regra de Negócio 3: Cria o registro de histórico para a criação
        var history = TaskHistory.ForCreation(task.Id, userId, task.Title);

        await _taskHistoryRepository.AddAsync(history);

        // Persiste todas as alterações no banco de dados em uma única transação
        await _unitOfWork.SaveChangesAsync();

        // Mapeia a entidade para um DTO de retorno
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            DueDate = task.DueDate,
            Priority = task.Priority,
            ProjectId = projectId,
            CreatedAt = DateTime.UtcNow
        };
    }

    public async Task<IEnumerable<TaskDto>> GetTasksByProjectAsync(Guid projectId, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se o projeto existe
        await ServiceHelper.CheckProject(projectId, _projectRepository);

        var tasks = await _taskRepository.GetByProjectIdAsync(projectId);

        // Mapear a lista de entidades para uma lista de TaskDto
        return tasks.Select(t => new TaskDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            DueDate = t.DueDate,
            Status = t.Status,
            Priority = t.Priority,
            ProjectId = t.ProjectId,
            CreatedAt = t.CreatedAt,
            UpdatedAt = t.UpdatedAt
        }).ToList();
    }

    public async Task<TaskDto?> GetTaskByIdAsync(Guid taskId, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se a tarefa existe
        var task = await ServiceHelper.CheckTask(taskId, _taskRepository);

        if (task == null) return null;

        // Mapeamento da entidade para TaskDto
        return new TaskDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            DueDate = task.DueDate,
            Status = task.Status,
            Priority = task.Priority,
            ProjectId = task.ProjectId,
            CreatedAt = task.CreatedAt,
            UpdatedAt = task.UpdatedAt
        };
    }

    /// <summary>
    /// Atualiza os detalhes de uma tarefa, registrando o histórico.
    /// </summary>
    public async Task UpdateTaskDetailsAsync(Guid taskId, UpdateTaskDto taskDto, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se a tarefa existe
        var task = await ServiceHelper.CheckTask(taskId, _taskRepository);

        if (task == null) return;

        // Regra de Negócio 3: Registrar histórico de alterações

        // Alteração do título da tarefa
        if (task.Title != taskDto.Title)
        {
            var history = TaskHistory.ForUpdate(taskId, userId, "Title", task.Title, taskDto.Title);
            await _taskHistoryRepository.AddAsync(history);
        }

        // Alteração da descriçãp da tarefa
        if (task.Description != taskDto.Description)
        {
            var history = TaskHistory.ForUpdate(taskId, userId, "Description", task.Description, taskDto.Description);
            await _taskHistoryRepository.AddAsync(history);
        }

        // Alteração da data de vencimento da tarefa
        if (task.DueDate != taskDto.DueDate)
        {
            var history = TaskHistory.ForUpdate(taskId, userId, "DueTo", task.DueDate.ToLongDateString(), taskDto.DueDate.ToLongDateString());
            await _taskHistoryRepository.AddAsync(history);
        }

        // Alteração do status da tarefa
        if (task.Status != taskDto.Status)
        {
            var history = TaskHistory.ForUpdate(taskId, userId, "Status", task.Status.ToString(), taskDto.Status.ToString());
            await _taskHistoryRepository.AddAsync(history);
        }

        // Atualiza a entidade usando o método da própria entidade
        task.UpdateDetails(taskDto.Title, taskDto.Description, taskDto.DueDate, taskDto.Status, DateTime.UtcNow);

        // Persiste todas as alterações no banco de dados em uma única transação
        await _unitOfWork.SaveChangesAsync();
    }

    /// <summary>
    /// Atualiza apenas o status de uma tarefa.
    /// </summary>
    public async Task UpdateTaskStatusAsync(Guid taskId, UpdateStatusDto statusDto, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se a tarefa existe
        var task = await ServiceHelper.CheckTask(taskId, _taskRepository);

        if (task == null) return;

        string oldStatus = task.Status.ToString();
        string newStatus = statusDto.Status.ToString();

        if (oldStatus != newStatus)
        {
            var history = TaskHistory.ForUpdate(taskId, userId, "Status", oldStatus, newStatus);

            if (history != null)
            {
                await _taskHistoryRepository.AddAsync(history);

                task.UpdateStatus(statusDto.Status,DateTime.UtcNow);

                // Persiste a alteração no banco de dados
                await _unitOfWork.SaveChangesAsync();
            }
        }
    }

    /// <summary>
    /// Adiciona um comentário a uma tarefa.
    /// </summary>
    public async Task AddCommentAsync(Guid taskId, AddCommentDto commentDto, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se a tarefa existe
        var task = await ServiceHelper.CheckTask(taskId, _taskRepository);

        if (task == null) return;

        // Regra de Negócio 6: Cria o registro de histórico para o comentário
        var history = TaskHistory.ForComment(taskId, userId, commentDto.Comment);

        await _taskHistoryRepository.AddAsync(history);

        // Persiste a alteração no banco de dados
        await _unitOfWork.SaveChangesAsync();
    }

    public async Task DeleteTaskAsync(Guid taskId, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se a tarefa existe
        var task = await ServiceHelper.CheckTask(taskId, _taskRepository);

        if (task == null) return;

        if (task != null)
        {
            // Procede com a remoção
            _taskRepository.Delete(task);

            // Persiste a alteração no banco de dados
            await _unitOfWork.SaveChangesAsync();
        }
    }

    /// <summary>
    /// Busca todo o histórico de uma tarefa específica.
    /// </summary>
    /// <param name="taskId">O ID da tarefa cujo histórico será recuperado.</param>
    /// <returns>Uma coleção de registros de histórico.</returns>
    public async Task<IEnumerable<TaskHistoryDto>> GetTaskHistoryAsync(Guid taskId, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Verifica se a tarefa existe
        var task = await ServiceHelper.CheckTask(taskId, _taskRepository);

        if (task == null) return new List<TaskHistoryDto>();

        var taskHistory = await _taskHistoryRepository.GetByTaskIdAsync(taskId);

        // Mapeia a lista de entidades para uma lista de DTOs
        return taskHistory.Select(h => new TaskHistoryDto
        {
            Id = h.Id,
            ChangeType = h.ChangeType,
            FieldName = h.FieldName,
            OldValue = h.OldValue,
            NewValue = h.NewValue,
            Comment = h.Comment,
            Timestamp = h.Timestamp,
            UserId = h.UserId
        }).ToList();
    }
}