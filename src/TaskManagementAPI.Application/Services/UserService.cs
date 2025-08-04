using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Services;

/// <summary>
/// Implementação da Interface para o serviço de Usuários.
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    /// <summary>
    /// Construtor do Serviço de Usuários.
    /// </summary>
    /// <param name="userRepository">Repositório de usuários.</param>
    /// <param name="uniOfWork">Controle de transações e operações.</param>
    public UserService(IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    /// <summary>
    /// Cria um novo usuário, de forma a facilitar o teste da aplicação.
    /// </summary>
    /// <param name="userDto">Os dados do novo usuário.</param>
    /// <returns>Os detalhes do usuário recém-criado.</returns>
    public async Task<UserDto> CreateUserAsync(UserDto userDto)
    {
        var user = new User
        {
            Name = userDto.Name,
            Email = userDto.Email,
            Role = userDto.Role
        };

        // Adciona o usuário
        await _userRepository.AddAsync(user);

        // Persiste a alteração no banco de dados
        await _unitOfWork.SaveChangesAsync();

        userDto.Id = user.Id;

        return userDto;
    }

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Uma lista de usuários cadastrados.</returns>
    public async Task<IEnumerable<UserDto>> GetUsersAsync()
    {
        // Obtém a lista de usuários cadastrados
        var users = await _userRepository.GetListAsync();

        // Mapeia a lista de entidades para uma lista de DTOs
        return users.Select(u => new UserDto
        {
            Id = u.Id,
            Name = u.Name,
            Email = u.Email,
            Role = u.Role
        }).ToList();
    }

    /// <summary>
    /// Busca um usuário específico pelo seu ID.
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

    /// <summary>
    /// Marca um usuário para deleção.
    /// <param name="userId">O ID do usuário a ser removido.</param>
    /// </summary>
    /// <returns>Nenhum conteúdo.</returns>
    public async Task DeleteUserAsync(Guid userId)
    {
        // Valida se o usuário existe
        var user = await _userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            // Lança uma exceção de usuário não encontrado
            throw new NotFoundException("Usuário não encontrado.");
        }

        // Procede com a remoção
        _userRepository.Delete(user);

        // Persiste a alteração no banco de dados em uma única transação
        await _unitOfWork.SaveChangesAsync();
    }
}