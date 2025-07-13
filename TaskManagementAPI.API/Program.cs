using System.Text;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services;
using TaskManagementAPI.Infrastructure.Data;
using TaskManagementAPI.Infrastructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// --- 1. CONFIGURAÇÃO DOS SERVIÇOS (Injeção de Dependência) ---

// Adiciona o DbContext para o PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Adiciona os Repositórios e a Unidade de Trabalho
// AddScoped: Uma instância é criada por requisição HTTP. Ideal para DbContext e repositórios.
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<ITaskRepository, TaskRepository>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITaskHistoryRepository, TaskHistoryRepository>();

// Adiciona os Serviços da Camada de Aplicação
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IReportService, ReportService>();

// Adiciona os serviços padrão da API
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Adiciona o conversor que lida com enums como strings
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

// --- 2. CONSTRUÇÃO E CONFIGURAÇÃO DO PIPELINE HTTP ---

var app = builder.Build();

app.UseHttpsRedirection();

// Mapeia as rotas para os controllers
app.MapControllers();

// Aplicar migrações automaticamente na inicialização
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        // Aplica quaisquer migrações pendentes ao banco de dados.
        // O banco de dados será criado se não existir.
        context.Database.Migrate(); 
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();

        logger.LogError(ex, "Ocorreu um erro durante a aplicação automática das migrações.");
    }
}

// Inicia a aplicação
app.Run();
