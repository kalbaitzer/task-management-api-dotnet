using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;

namespace TaskManagementAPI.Application.Services;

/// <summary>
/// Implementação do serviço de perfis de usuário.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;

    // Note a ausência de IPasswordHasher e IJwtTokenGenerator.
    public UserService(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    /// <summary>
    /// Busca o perfil de um usuário pelo seu ID.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser buscado.</param>
    /// <returns>Um DTO com os dados do usuário.</returns>
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            // Lançar uma exceção é uma opção, mas para uma simples busca,
            // retornar nulo permite que o controller decida como responder (ex: 404 Not Found).
            return null;
        }

        // Mapeia a entidade para um DTO seguro para evitar expor dados internos.
        return new UserDto
        {
            Id = user.Id,
            Name = user.Name,
            Email = user.Email,
            Role = user.Role
        };
    }
}