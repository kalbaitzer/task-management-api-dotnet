using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services.Utils;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Services;

/// <summary>
/// Implementação da Interface para o serviço de Projetos.
/// </summary>
public class ProjectService : IProjectService
{
    private readonly IProjectRepository _projectRepository;
    private readonly ITaskRepository _taskRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Construtor do Serviço de Projetos.
    /// </summary>
    /// <param name="projectRepository">Repositório de projetos.</param>
    /// <param name="taskRepository">Repositório de tarefas.</param>
    /// <param name="userRepository">Repositório de usuários.</param>
    /// <param name="uniOfWork">Controle de transações e operações.</param>
    public ProjectService(IProjectRepository projectRepository, ITaskRepository taskRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _projectRepository = projectRepository;
        _taskRepository = taskRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria um novo projeto para o usuário autenticado.
    /// </summary>
    /// <param name="projectDto">Os dados do novo projeto.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Os detalhes do projeto recém-criado.</returns>
     public async Task<ProjectDetailDto> CreateProjectAsync(CreateProjectDto projectDto, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        var project = new Project
        {
            Name = projectDto.Name,
            Description = projectDto.Description,
            OwnerUserId = userId // Define o proprietário do projeto
        };

        // Adiciona o projeto
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
    /// Busca um projeto pelo seu ID
    /// </summary>
    /// <param name="projetcId">O ID do projeto.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>A entidade do projeto ou nulo se não for encontrada.</returns>
    public async Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Obtém o projeto do repositório
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
    /// Lista todos os projetos cadastrados.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>A entidade do projeto com as tarefas carregadas.</returns>
    public async Task<ProjectDetailDto?> GetProjectByIdAsync(Guid projectId, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

        // Obtém a lista de projetos
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
    /// <param name="projectId">O ID do projeto a ser removido.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Nenhum conteúdo.</returns>
    public async Task DeleteProjectAsync(Guid projectId, Guid userId)
    {
        // Verifica se o usuário existe
        await ServiceHelper.CheckUser(userId, _userRepository);

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