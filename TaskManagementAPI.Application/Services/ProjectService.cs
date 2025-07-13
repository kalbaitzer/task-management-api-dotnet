using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.Application.Services;

public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository; // Dependência crucial para a Regra de Negócio 2
    private readonly IUnitOfWork _unitOfWork;

    public ProjectService(
        IProjectRepository projectRepository,
        ITaskRepository taskRepository,
        IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria um novo projeto para o usuário especificado.
    /// </summary>
    public async Task<ProjectDetailDto> CreateProjectAsync(CreateProjectDto projectDto, Guid userId)
    {
        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description,
            OwnerUserId = userId // Define o proprietário do projeto
        };

        await _projectRepository.AddAsync(project);

        // Persiste a alteração no banco de dados
        await _unitOfWork.SaveChangesAsync();

        // Mapeia a entidade para um DTO de retorno detalhado
        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,
            Tasks = new List<TaskProjectDto>() // Novo projeto começa sem tarefas
        };
    }

    /// <summary>
    /// Lista todos os projetos de um usuário.
    /// </summary>
    public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(Guid userId)
    {
        var projects = await _projectRepository.GetByOwnerIdAsync(userId);

        // Mapeia a lista de entidades para uma lista de DTOs de resumo
        return projects.Select(p => new ProjectDto
        {
            Id = p.Id,
            Name = p.Name,
            TaskCount = p.Tasks.Count
        }).ToList();
    }

    /// <summary>
    /// Busca um projeto detalhado por ID, verificando a propriedade.
    /// </summary>
    public async Task<ProjectDetailDto?> GetProjectByIdAsync(Guid projectId, Guid userId)
    {
        var project = await _projectRepository.GetByIdWithTasksAsync(projectId);

        if (project == null)
        {
            // Lança uma exceção de projeto não encontrado
            throw new NotFoundException("Projeto não encontrado.");
        }
        
        // Mapeia a entidade e suas tarefas para o DTO detalhado
        return new ProjectDetailDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            CreatedAt = project.CreatedAt,

            Tasks = project.Tasks.Select(t => new TaskProjectDto
            {
                Id = t.Id,
                Title = t.Title,
                Description = t.Description,
                Status = t.Status,
                Priority = t.Priority,
                DueDate = t.DueDate,
                CreatedAt = t.CreatedAt,
                UpdatedAt = t.UpdatedAt
            }).ToList()
        };
    }

    /// <summary>
    /// Remove um projeto, aplicando a regra de negócio de restrição de remoção.
    /// </summary>
    public async System.Threading.Tasks.Task DeleteProjectAsync(Guid projectId, Guid userId)
    {
        // Valida se o projeto existe e se o usuário é o proprietário
        var project = await _projectRepository.GetByIdAsync(projectId);

        if (project == null)
        {
            // Lança uma exceção de projeto não encontrado
            throw new NotFoundException("Projeto não encontrado.");
        }

        // Regra de Negócio 2: Verificar se existem tarefas pendentes ou em andamento
        var hasActiveTasks = await _taskRepository.HasActiveTasksInProjectAsync(projectId);

        if (hasActiveTasks)
        {
            // Lança uma exceção de regrade negócio
            throw new BusinessRuleException("Não é possível remover o projeto. Existem tarefas pendentes ou em andamento. Conclua ou remova as tarefas primeiro.");
        }

        // Procede com a remoção
        _projectRepository.Delete(project);

        // Persiste a alteração no banco de dados em uma única transação
        await _unitOfWork.SaveChangesAsync();
    }
}