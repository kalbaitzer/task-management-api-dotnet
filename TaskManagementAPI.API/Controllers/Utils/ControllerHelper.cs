using System.Net;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.API.Controllers.Utils;

/// <summary>
/// Classe Utilitária para validações e tratamento de exceções nos controllers
/// </summary>
public static class ControllerHelper
{
    public static async Task<UserDto?> CheckUser(HttpRequest request, IUserService userService)
    {
        if (!request.Headers.TryGetValue("X-User-Id", out var userIdStr) || !Guid.TryParse(userIdStr, out var userId))
        {
            // Retorna um erro se o cabeçalho essencial não for fornecido.
            throw new NotFoundException("Usuário ausente ou inválido.");
        }

        var user = await userService.GetUserByIdAsync(userId);

        if (user == null)
        {
            // Retorna um erro se o usuário não estiver cadastrado.
            throw new NotFoundException("Usuário não cadastrado.");
        }

        return user;
    }

    public static async Task<UserDto?> CheckManager(HttpRequest request, IUserService userService)
    {
        // Verificação do usuário
        var user = await ControllerHelper.CheckUser(request, userService);

        if (user != null)
        {
            if (user.Role != "Manager")
            {
                // Retorna HTTP 403 Forbidden
                throw new ForbiddenAccessException("Você não tem permissão para acessar este relatório.");
            }
        }

        return user;
    }

    public static int GetStatusCode(Exception exception)
    {
        if (exception != null)
        {
            if (exception is BusinessRuleException) return StatusCodes.Status400BadRequest;
            else
            if (exception is ForbiddenAccessException) return StatusCodes.Status403Forbidden;
            else
            if (exception is NotFoundException) return StatusCodes.Status404NotFound;
        }

        return StatusCodes.Status500InternalServerError;
    }
}