using TaskManagementAPI.Application.DTOs;

namespace TaskManagementAPI.Application.Interfaces;

/// <summary>
/// Interface para o serviço de Projetos.
/// </summary>
public interface IProjectService
{
    /// <summary>
    /// Cria um novo projeto para o usuário autenticado.
    /// </summary>
    /// <param name="projectDto">Os dados do novo projeto.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Os detalhes do projeto recém-criado.</returns>
    Task<ProjectDetailDto> CreateProjectAsync(CreateProjectDto projectDto, Guid userId);

    /// <summary>
    /// Busca um projeto pelo seu ID
    /// </summary>
    /// <param name="projetcId">O ID do projeto.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>A entidade do projeto ou nulo se não for encontrada.</returns>
    Task<ProjectDetailDto?> GetProjectByIdAsync(Guid projectId, Guid userId);

    /// <summary>
    /// Lista todos os projetos cadastrados.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>A entidade do projeto com as tarefas carregadas.</returns>
    Task<IEnumerable<ProjectDto>> GetUserProjectsAsync(Guid userId);

    /// <summary>
    /// Remove um projeto, aplicando a regra de negócio de restrição de remoção.
    /// </summary>
    /// <param name="projectId">O ID do projeto a ser removido.</param>
    /// <param name="userIdd">O ID do usuário.</param>
    /// <returns>Nenhum conteúdo.</returns>
    Task DeleteProjectAsync(Guid projectId, Guid userId);
}