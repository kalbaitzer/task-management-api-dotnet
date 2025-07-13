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
    /// Busca um usuário pelo seu endereço de e-mail de forma 'case-insensitive'.
    /// </summary>
    /// <param name="email">O e-mail a ser pesquisado.</param>
    /// <returns>A entidade User ou nulo se não for encontrada.</returns>
    public async Task<User?> GetByEmailAsync(string email)
    {
        // FirstOrDefaultAsync é usado porque esperamos no máximo um resultado,
        // já que o e-mail é uma chave única.
        // Comparar em minúsculas garante uma busca case-insensitive, que é o
        // comportamento esperado para e-mails.
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower());
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
}