namespace TaskManagementAPI.Application.Exceptions;

/// <summary>
/// Representa erros que ocorrem quando um usuário autenticado tenta acessar
/// um recurso ou executar uma ação para a qual não tem permissão.
/// Esta exceção deve ser usada para falhas de autorização.
/// Resulta em uma resposta HTTP 403 Forbidden.
/// </summary>
[Serializable]
public class ForbiddenAccessException : Exception
{
    /// <summary>
    /// Cria uma nova instância da ForbiddenAccessException.
    /// </summary>
    public ForbiddenAccessException()
    {
    }

    /// <summary>
    /// Cria uma nova instância da ForbiddenAccessException com uma mensagem de erro específica.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    public ForbiddenAccessException(string message) : base(message)
    {
    }

    /// <summary>
    /// Cria uma nova instância da ForbiddenAccessException com uma mensagem de erro específica
    /// e uma referência para a exceção interna que é a causa do erro.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    /// <param name="innerException">A exceção que é a causa da exceção atual.</param>
    public ForbiddenAccessException(string message, Exception innerException) : base(message, innerException)
    {
    }
}