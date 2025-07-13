namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para expor os dados públicos e seguros de um usuário.
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}