using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.Application.Interfaces.Repositories;

/// <summary>
/// Interface para o repositório de Usuários.
/// Define o contrato para as operações de dados relacionadas à entidade User,
/// com foco principal em suportar os processos de autenticação e registro.
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Busca um usuário pelo seu endereço de e-mail.
    /// Este é um método crucial e deve ser otimizado para performance.
    /// Usado para verificar se um e-mail já existe durante o registro e para
    /// encontrar o usuário durante a autenticação.
    /// </summary>
    /// <param name="email">O e-mail do usuário a ser buscado.</param>
    /// <returns>A entidade User ou nulo se não for encontrada.</returns>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Busca um usuário pelo seu ID único.
    /// Útil para buscar dados de um usuário já autenticado ou para futuras
    /// funcionalidades administrativas.
    /// </summary>
    /// <param name="id">O ID do usuário.</param>
    /// <returns>A entidade User ou nulo se não for encontrada.</returns>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Adiciona um novo usuário ao contexto do banco de dados.
    /// Usado pelo UserService no processo de registro de um novo usuário.
    /// </summary>
    /// <param name="user">A entidade User a ser adicionada.</param>
    System.Threading.Tasks.Task AddAsync(User user);
}