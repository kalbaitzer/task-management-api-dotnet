using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;
using TaskManagementAPI.Infrastructure.Data;

namespace TaskManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do repositório de Usuários, usando Entity Framework Core.
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _context;

    public UserRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Adiciona um novo usuário ao contexto do banco de dados.
    /// A persistência real ocorre quando SaveChangesAsync é chamado na Unidade de Trabalho.
    /// </summary>
    /// <param name="user">A entidade User a ser adicionada.</param>
    public async System.Threading.Tasks.Task AddAsync(User user)
    {
        await _context.Users.AddAsync(user);
    }

    /// <summary>
    /// Busca todos os usuários cadastrados.
    /// </summary>
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
    /// <param name="user">A entidade User a ser removida.</param>
    /// </summary>
    public void Delete(User user)
    {
        _context.Users.Remove(user);
    }
}