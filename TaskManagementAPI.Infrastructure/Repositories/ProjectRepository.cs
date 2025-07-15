using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;
using TaskManagementAPI.Infrastructure.Data;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório de Projetos, usando Entity Framework Core.
/// </summary>
public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do repositório de Projetos.
    /// </summary>
    /// <param name="context">Núcleo do Entity FRamework Core que interage com o banco de dados.</param>
    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo projeto ao contexto do banco de dados.
    /// </summary>
    /// <param name="project">A entidade do projeto a ser adicionada.</param>
    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    /// <summary>
    /// Busca um projeto pelo seu ID, sem incluir dados relacionados como tarefas.
    /// Útil para operações leves como verificações de existência ou deleção.
    /// </summary>
    /// <param name="id">O ID do projeto.</param>
    /// <returns>A entidade do projeto ou nulo se não for encontrada.</returns>
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects.FindAsync(id);
    }

    /// <summary>
    /// Busca um projeto pelo seu ID, incluindo sua lista de tarefas associadas.
    /// Usado pelo ProjectService para obter a visualização detalhada de um projeto.
    /// </summary>
    /// <param name="id">O ID do projeto.</param>
    /// <returns>A entidade do projeto com as tarefas carregadas.</returns>
    public async Task<Project?> GetByIdWithTasksAsync(Guid id)
    {
        // Usamos Include() para "eager loading" (carregamento adiantado) das tarefas.
        // FindAsync não pode ser usado com Include, por isso usamos FirstOrDefaultAsync.
        return await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Busca todos os projetos pertencentes a um usuário específico.
    /// Usado pelo ProjectService para a funcionalidade de "listar meus projetos".
    /// </summary>
    /// <param name="ownerId">O ID do usuário proprietário.</param>
    /// <returns>Uma coleção de projetos do usuário.</returns>
    public async Task<IEnumerable<Project>> GetByOwnerIdAsync(Guid ownerId)
    {
        // Incluímos as tarefas para que a camada de serviço possa facilmente
        // calcular a contagem de tarefas para o DTO.
        // Ordenamos por data de criação para uma melhor experiência de usuário (mais recentes primeiro).
        return await _context.Projects
            .Include(p => p.Tasks)
            .Where(p => p.OwnerUserId == ownerId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();
    }

    /// <summary>
    /// Marca um projeto para remoção do banco de dados.
    /// </summary>
    /// <param name="project">A entidade do projeto a ser removida.</param>
     /// <returns>Nenhum conteúdo.</returns>
   public void Delete(Project project)
    {
        _context.Projects.Remove(project);
    }
}