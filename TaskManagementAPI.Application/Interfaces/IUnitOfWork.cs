namespace TaskManagementAPI.Application.Interfaces;

/// <summary>
/// Define o contrato para a Unidade de Trabalho (Unit of Work).
/// Este padrão é usado para agrupar uma ou mais operações de repositório
/// em uma única transação, garantindo a consistência dos dados.
/// A implementação desta interface irá coordenar o salvamento das alterações
/// feitas nos repositórios.
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Salva todas as alterações feitas no contexto da unidade de trabalho para o
    /// banco de dados de forma assíncrona.
    /// Esta é a chamada que efetivamente "commita" a transação.
    /// </summary>
    /// <param name="cancellationToken">Um token para observar solicitações de cancelamento.</param>
    /// <returns>O número de registros de estado escritos no banco de dados.</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}