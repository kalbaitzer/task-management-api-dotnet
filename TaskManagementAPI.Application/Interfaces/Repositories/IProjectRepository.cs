using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de Projetos. Define o contrato para as operações de dados
/// relacionadas a projetos.
/// </summary>
public interface IProjectRepository
{
    // --- Métodos de Busca ---

    /// <summary>
    /// Busca um projeto pelo seu ID, sem incluir dados relacionados como tarefas.
    /// Útil para operações leves como verificações de existência ou deleção.
    /// </summary>
    /// <param name="id">O ID do projeto.</param>
    /// <returns>A entidade do projeto ou nulo se não for encontrada.</returns>
    Task<Project?> GetByIdAsync(Guid id);

    /// <summary>
    /// Busca um projeto pelo seu ID, incluindo sua lista de tarefas associadas.
    /// Usado pelo ProjectService para obter a visualização detalhada de um projeto.
    /// </summary>
    /// <param name="id">O ID do projeto.</param>
    /// <returns>A entidade do projeto com as tarefas carregadas.</returns>
    Task<Project?> GetByIdWithTasksAsync(Guid id);

    /// <summary>
    /// Busca todos os projetos pertencentes a um usuário específico.
    /// Usado pelo ProjectService para a funcionalidade de "listar meus projetos".
    /// </summary>
    /// <param name="ownerId">O ID do usuário proprietário.</param>
    /// <returns>Uma coleção de projetos do usuário.</returns>
    Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId);


    // --- Métodos de Escrita ---

    /// <summary>
    /// Adiciona um novo projeto ao contexto do banco de dados.
    /// </summary>
    /// <param name="project">A entidade do projeto a ser adicionada.</param>
    System.Threading.Tasks.Task AddAsync(Project project);

    /// <summary>
    /// Marca um projeto para remoção do banco de dados.
    /// </summary>
    /// <param name="project">A entidade do projeto a ser removida.</param>
    void Delete(Project project);
}