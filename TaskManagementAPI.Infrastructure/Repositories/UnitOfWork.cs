using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Infrastructure.Data;

namespace TaskManagementAPI.Infrastructure.Repositories;

/// <summary>
/// Implementação concreta do padrão Unit of Work.
/// Encapsula o DbContext para garantir que todas as operações de repositório
/// dentro de uma única requisição compartilhem o mesmo contexto e transação.
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Uma referência para o DbContext do Entity Framework Core.
    /// É 'private' e 'readonly' para garantir que seja atribuído apenas uma vez via construtor.
    /// </summary>
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// O construtor recebe uma instância do ApplicationDbContext através de injeção de dependência.
    /// Como tanto o UnitOfWork quanto o DbContext são registrados como 'Scoped' no Program.cs,
    /// esta será a mesma instância do DbContext para toda a requisição HTTP.
    /// </summary>
    /// <param name="context">O DbContext da aplicação.</param>
    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Implementação do método SaveChangesAsync da interface IUnitOfWork.
    /// Apenas delega a chamada para o método SaveChangesAsync do DbContext,
    /// que é a implementação nativa do Unit of Work do EF Core.
    /// </summary>
    /// <param name="cancellationToken">O token de cancelamento.</param>
    /// <returns>O número de registros de estado escritos no banco de dados.</returns>
    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Implementação do método Dispose da interface IDisposable.
    /// Garante que o DbContext seja devidamente descartado ao final do escopo da requisição,
    /// liberando a conexão com o banco de dados e outros recursos.
    /// </summary>
    public void Dispose()
    {
        _context.Dispose();
    }
}