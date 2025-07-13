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
}