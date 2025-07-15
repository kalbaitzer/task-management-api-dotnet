using TaskManagementAPI.Application.DTOs;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Interfaces;

/// <summary>
/// Interface para o serviço de Usuários.
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Cria um novo usuário, de forma a facilitar o teste da aplicação.
    /// </summary>
    /// <param name="userDto">Os dados do novo usuário.</param>
    /// <returns>Os detalhes do usuário recém-criado.</returns>
    Task<UserDto> CreateUserAsync(UserDto userDto);

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Uma lista de usuários cadastrados.</returns>
    Task<IEnumerable<UserDto>> GetUsersAsync();

    /// <summary>
    /// Busca um usuário específico pelo seu ID.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser buscado.</param>
    /// <returns>Um DTO com os dados do usuário.</returns>
    Task<UserDto?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Marca um usuário para deleção.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser removido.</param>
    /// <returns>Nenhum conteúdo.</returns>
    Task DeleteUserAsync(Guid userId);
}