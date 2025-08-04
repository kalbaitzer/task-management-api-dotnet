namespace TaskManagementAPI.Application.Exceptions;

/// <summary>
/// Representa erros que ocorrem quando uma regra de negócio da aplicação é violada.
/// Esta exceção deve ser usada para sinalizar falhas que podem ser entendidas
/// e potencialmente corrigidas pelo usuário da API (ex: tentar adicionar uma tarefa
/// além do limite). Geralmente, resulta em uma resposta HTTP 400 Bad Request.
/// </summary>
[Serializable]
public class BusinessRuleException : Exception
{
    /// <summary>
    /// Cria uma nova instância da BusinessRuleException.
    /// </summary>
    public BusinessRuleException()
    {
    }

    /// <summary>
    /// Cria uma nova instância da BusinessRuleException com uma mensagem de erro específica.
    /// Este é o construtor mais comumente usado.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    public BusinessRuleException(string message) : base(message)
    {
    }

    /// <summary>
    /// Cria uma nova instância da BusinessRuleException com uma mensagem de erro específica
    /// e uma referência para a exceção interna que é a causa do erro.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    /// <param name="innerException">A exceção que é a causa da exceção atual.</param>
    public BusinessRuleException(string message, Exception innerException) : base(message, innerException)
    {
    }
}