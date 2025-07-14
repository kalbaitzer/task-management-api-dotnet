using TaskManagementAPI.Application.DTOs;

namespace TaskManagementAPI.Application.Interfaces;

/// <summary>
/// Contrato para o serviço que gerencia a lógica de negócio de perfis de usuário.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Busca o perfil de um usuário pelo seu ID.
    /// </summary>
    /// <param name="userId">O ID do usuário.</param>
    /// <returns>Um DTO com os dados do usuário ou nulo se não for encontrado.</returns>
    Task<UserDto?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Adiciona um novo usuário ao contexto do banco de dados.
    /// </summary>
    /// <param name="user">A entidade User a ser adicionada.</param>
    /// <returns>Um DTO com os dados do usuário inserido.</returns>
    Task<UserDto> CreateUserAsync(UserDto userDto);

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Uma lista de usuários cadastrados.</returns>
    Task<IEnumerable<UserDto>> GetUsersAsync();

    /// <summary>
    /// Marca um usuário para deleção.
    /// <param name="userId">O ID do usuário a ser removido.</param>
    /// </summary>
    System.Threading.Tasks.Task DeleteUserAsync(Guid userId);
}