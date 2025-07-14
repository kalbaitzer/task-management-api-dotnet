using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.API.Controllers.Utils;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Interfaces;

namespace TaskManagementAPI.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os endpoints relacionados a Usuários.
/// </summary>
[ApiController]
[Route("api/[controller]")] // Rota base: /api/users
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    /// <summary>
    /// Cria um novo usuário, de forma a facilitar o teste da aplicação.
    /// </summary>
    /// <param name="userDto">Os dados do novo usuário.</param>
    /// <returns>Os detalhes do usuário recém-criado.</returns>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
    {
        try
        {
            var newUser = await _userService.CreateUserAsync(userDto);

            if (newUser != null)
            {
                // Retorna o novo usuário criado
                return CreatedAtAction(nameof(GetUserById), new { userId = newUser.Id }, newUser);
            }
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Lista todos os usuários cadastrados.
    /// </summary>
    /// <returns>Uma lista de usuários.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUsers()
    {
        try
        {
            var users = await _userService.GetUsersAsync();

            if (users != null) return Ok(users);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Busca um usuário específico pelo seu ID.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser buscado.</param>
    /// <returns>Os detalhes do usuário.</returns>
    [HttpGet("{userId}")]
    [ProducesResponseType(typeof(UserDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserById(Guid userId)
    {
        try
        {
            var user = await _userService.GetUserByIdAsync(userId);

            if (user != null) return Ok(user);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NotFound(); 
    }

    /// <summary>
    /// Remove um usuário específico.
    /// </summary>
    /// <param name="userId">O ID do usuário a ser removido.</param>
    /// <returns>Nenhum conteúdo.</returns>
    [HttpDelete("{userId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteUser(Guid userId)
    {
        try
        {
            await _userService.DeleteUserAsync(userId);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        // HTTP 204 No Content é a resposta padrão para uma deleção bem-sucedida.
        return NoContent();
    }
}