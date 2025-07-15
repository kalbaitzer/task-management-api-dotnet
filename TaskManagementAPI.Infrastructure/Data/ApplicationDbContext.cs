using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Core.Entities;

// Usando um alias para a entidade Task para evitar conflito de nome
using TaskEntity = TaskManagementAPI.Core.Entities.Task;

namespace TaskManagementAPI.Infrastructure.Data;

/// <summary>
/// O DbContext da aplicação, que representa uma sessão com o banco de dados
/// e permite consultar e salvar instâncias das suas entidades.
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Construtor que recebe as opções de configuração do DbContext,
    /// como a string de conexão. Essas opções são injetadas pelo
    /// contêiner de Injeção de Dependência do ASP.NET Core.
    /// </summary>
    /// <param name="options">As opções para configurar o contexto.</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    // --- DbSet Properties ---
    // Cada DbSet representa uma tabela no banco de dados que pode ser consultada.

    public DbSet<User> Users { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<TaskEntity> Tasks { get; set; }
    public DbSet<TaskHistory> TaskHistories { get; set; }

    /// <summary>
    /// Este método é chamado pelo Entity Framework Core quando o modelo para um
    /// contexto derivado está sendo criado. É aqui que você pode usar a
    /// "Fluent API" para configurar o modelo (ex: chaves, índices, relacionamentos).
    /// </summary>
    /// <param name="modelBuilder">O construtor que está sendo usado para construir o modelo.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Chama a implementação base primeiro, que pode ter suas próprias configurações.
        base.OnModelCreating(modelBuilder);

        // Garante que a extensão uuid-ossp, que contém a função uuid_generate_v4(),
        // seja criada no banco de dados, se não existir.
        modelBuilder.HasPostgresExtension("uuid-ossp");

        // --- Configurações de Entidades com a Fluent API ---

        // Configuração da Entidade User
        modelBuilder.Entity<User>(entity =>
        {
            // Informa que o campo Id será gerado automaticamente pelo banco de dados
            entity.Property(u => u.Id).HasDefaultValueSql("uuid_generate_v4()");

            // Garante que o campo Email seja único no banco de dados.
            // Isso cria um índice único na tabela Users, prevenindo a inserção
            // de dois usuários com o mesmo e-mail.
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Configuração da Entidade Project
        modelBuilder.Entity<Project>(entity =>
        {
            // Informa que o campo Id será gerado automaticamente pelo banco de dados
            entity.Property(p => p.Id).HasDefaultValueSql("uuid_generate_v4()");

            // Define o relacionamento de um-para-muitos: Um User (Owner) tem muitos Projects.
            // Define o comportamento de exclusão: Se um User for deletado, seus projetos
            // não serão deletados em cascata (Restrict), para evitar perda acidental de dados.
            // A exclusão de um usuário deve ser uma operação controlada.
            entity.HasOne(p => p.Owner)
                  .WithMany(u => u.Projects)
                  .HasForeignKey(p => p.OwnerUserId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Configuração da Entidade Task
        modelBuilder.Entity<TaskEntity>(entity =>
        {
            // Informa que o campo Id será gerado automaticamente pelo banco de dados
            entity.Property(t => t.Id).HasDefaultValueSql("uuid_generate_v4()");

            // Define o relacionamento: Uma Task tem um Project.
            // Define o comportamento de exclusão: Se um Project for deletado,
            // todas as suas Tasks associadas serão deletadas em cascata (Cascade).
            // Isso faz sentido, pois uma tarefa não pode existir sem um projeto.
            entity.HasOne(t => t.Project)
                  .WithMany(p => p.Tasks)
                  .HasForeignKey(t => t.ProjectId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configuração da Entidade TaskHistory
        modelBuilder.Entity<TaskHistory>(entity =>
        {
            // Informa que o campo Id será gerado automaticamente pelo banco de dados
            entity.Property(th => th.Id).HasDefaultValueSql("uuid_generate_v4()");

            // Define o relacionamento: Um TaskHistory pertence a uma Task.
            // Se a Task for deletada, todo o seu histórico será deletado junto.
            entity.HasOne(th => th.Task)
                  .WithMany(t => t.TaskHistories)
                  .HasForeignKey(th => th.TaskId)
                  .OnDelete(DeleteBehavior.Cascade);
        });
    }
}