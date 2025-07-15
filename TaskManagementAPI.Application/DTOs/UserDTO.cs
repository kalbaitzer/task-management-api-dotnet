namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber os dados de um usuário.
/// </summary>
public class UserDto
{
    /// <summary>
    /// Identificador único do usuário.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Nome do usuário.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Email do usuário.
    /// </summary>
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Role do usuário: "User" ou "Manager".
    /// </summary>
    public string Role { get; set; } = string.Empty;
}