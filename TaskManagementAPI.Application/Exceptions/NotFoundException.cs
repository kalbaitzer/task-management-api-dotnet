namespace TaskManagementAPI.Application.Exceptions;

/// <summary>
/// Representa erros que ocorrem quando um recurso ou entidade específica não é encontrada.
/// Esta exceção deve ser usada para sinalizar que uma busca por um item específico
/// (ex: por ID) não retornou resultados. Resulta em uma resposta HTTP 404 Not Found.
/// </summary>
[Serializable]
public class NotFoundException : Exception
{
    /// <summary>
    /// Cria uma nova instância da NotFoundException.
    /// </summary>
    public NotFoundException()
    {
    }

    /// <summary>
    /// Cria uma nova instância da NotFoundException com uma mensagem de erro específica.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    public NotFoundException(string message) : base(message)
    {
    }

    /// <summary>
    /// Cria uma nova instância da NotFoundException com o nome da entidade e sua chave.
    /// </summary>
    /// <param name="name">O nome da entidade (ex: "Tarefa").</param>
    /// <param name="key">A chave ou ID que não foi encontrado.</param>
    public NotFoundException(string name, object key)
        : base($"Entidade \"{name}\" ({key}) não foi encontrada.")
    {
    }

    /// <summary>
    /// Cria uma nova instância da NotFoundException com uma mensagem de erro específica
    /// e uma referência para a exceção interna que é a causa do erro.
    /// </summary>
    /// <param name="message">A mensagem que descreve o erro.</param>
    /// <param name="innerException">A exceção que é a causa da exceção atual.</param>
    public NotFoundException(string message, Exception innerException) : base(message, innerException)
    {
    }
}