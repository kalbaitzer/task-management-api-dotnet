using TaskManagementAPI.Application.Exceptions;

namespace TaskManagementAPI.API.Controllers.Utils;

/// <summary>
/// Classe Utilitária para validações e tratamento de exceções nos controllers
/// </summary>
public static class ControllerHelper
{
    public static Guid GetUserId(HttpRequest request)
    {
        if (request.Headers.TryGetValue("X-User-Id", out var userIdStr))
        {
            if (Guid.TryParse(userIdStr, out var userId))
            {
                return userId;
            }
        }

        return Guid.Empty; 
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