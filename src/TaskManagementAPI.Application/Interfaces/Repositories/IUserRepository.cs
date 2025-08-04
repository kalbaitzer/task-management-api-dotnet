using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de Usuários.
/// Define o contrato para as operações de dados relacionadas à entidade User,
/// com foco principal em suportar os processos de autenticação e registro.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Adiciona um novo usuário ao contexto do banco de dados.
    /// Usado pelo UserService no processo de registro de um novo usuário.
    /// </summary>
    /// <param name="user">A entidade User a ser adicionada.</param>
    Task AddAsync(User user);

    /// <summary>
    /// Busca todos os usuários cadastrados.
    /// </summary>
    /// <returns>A lista de usuários cadastrados.</returns>
    Task<IEnumerable<User>> GetListAsync();

    /// <summary>
    /// Busca um usuário pelo seu ID único.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>A entidade User ou nulo se não for encontrada.</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Marca um usuário para deleção.
    /// <param name="user">A entidade User a ser removida.</param>
    /// </summary>
    void Delete(User user);
}