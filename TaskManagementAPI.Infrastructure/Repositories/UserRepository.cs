using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;
using TaskManagementAPI.Infrastructure.Data;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório de Usuários, usando Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Construtor do repositório de Usuários.
    /// </summary>
    /// <param name="context">Núcleo do Entity FRamework Core que interage com o banco de dados.</param>
    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Cria um novo usuário, de forma a facilitar o teste da aplicação.
    /// </summary>
    /// <param name="user">Os dados do novo usuário.</param>
    /// <returns>Os detalhes do usuário recém-criado.</returns>
     public async Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Uma lista de usuários cadastrados.</returns>
    public async Task<IEnumerable<User>> GetListAsync()
    {
        // Obtém a lista de usuários cadastrados ordenados pelo nome.
        return await _context.Users
            .OrderByDescending(u => u.Name)
            .ToListAsync();
    }

    /// <summary>
    /// Busca um usuário pelo seu ID de forma otimizada.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>A entidade User ou nulo se não for encontrada.</returns>
    public async Task<User?> GetByIdAsync(Guid id)
    {
        // FindAsync é o método mais eficiente para buscar uma entidade pela sua chave primária.
        return await _context.Users.FindAsync(id);
    }

    /// <summary>
    /// Marca um usuário para deleção.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser removido.</param>
    /// <returns>Nenhum conteúdo.</returns>
    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }
}