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

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo projeto ao contexto.
    /// </summary>
    public async Task AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
    }

    /// <summary>
    /// Marca um projeto para deleção.
    /// </summary>
    public void Delete(Project project)
    {
        _context.Projects.Remove(project);
    }

    /// <summary>
    /// Busca um projeto pelo seu ID, sem carregar dados relacionados.
    /// </summary>
    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects.FindAsync(id);
    }

    /// <summary>
    /// Busca um projeto pelo ID, incluindo a lista de tarefas associadas.
    /// </summary>
    public async Task<Project?> GetByIdWithTasksAsync(Guid id)
    {
        // Usamos Include() para "eager loading" (carregamento adiantado) das tarefas.
        // FindAsync não pode ser usado com Include, por isso usamos FirstOrDefaultAsync.
        return await _context.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    /// <summary>
    /// Busca todos os projetos de um usuário específico, incluindo as tarefas para contagem.
    /// </summary>
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
}