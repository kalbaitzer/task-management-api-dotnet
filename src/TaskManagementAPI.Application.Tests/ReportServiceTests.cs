using Moq;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Tests;

/// <summary>
/// Classe de testes para o serviço de Relatórios
/// </summary>
public class ReportServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly ReportService _reportService;

    /// <summary>
    /// O construtor da classe de testes para o serviço de Relatórios
    /// </summary>
    public ReportServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _reportService = new ReportService(_taskRepositoryMock.Object,_userRepositoryMock.Object);
    }

    // --- TESTE PARA A REGRA DE NEGÓCIO 5 ---

    /// <summary>
    /// Teste: relatórico gerado com sucesso quando as tarefas existem e com  o cálculo correto de média de tarefas
    /// concluídas por usuário.
    /// </summary>
    [Fact]
    public async Task GenerateAverageCompletedTasksReportAsync_ShouldReturnCorrectAverage_WhenTasksExist()
    {
        // Arrange (Arranjar)
        var userId = Guid.NewGuid();
        var userAId = Guid.NewGuid();
        var userBId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId, Role = "Manager" };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Cria uma lista de tarefas "falsas" para simular o retorno do banco.
        // Usuário A completou 3 tarefas. Usuário B completou 1 tarefa.
        // Total: 4 tarefas, 2 usuários distintos. Média esperada: 4 / 2 = 2.0
        var fakeTasks = new List<TaskHistory>
        {
            // Tarefas do Usuário A
            new TaskHistory("Update", "Status", "Tarefa 1", "Concluida", "", Guid.NewGuid(), userAId),
            new TaskHistory("Update", "Status", "Tarefa 2", "Concluida", "", Guid.NewGuid(), userAId),
            new TaskHistory("Update", "Status", "Tarefa 3", "Concluida", "", Guid.NewGuid(), userAId),

            // Tarefa do Usuário B
            new TaskHistory("Update", "Status", "Tarefa 4", "Concluida", "", Guid.NewGuid(), userBId),
        };

        // Configura o mock do repositório para retornar nossa lista falsa.
        _taskRepositoryMock.Setup(repo => repo.GetCompletedTasksSinceAsync(It.IsAny<DateTime>()))
                           .ReturnsAsync(fakeTasks);

        // Act (Agir)
        var report = await _reportService.GenerateAverageCompletedTasksReportAsync(userId);

        // Assert (Afirmar)
        Assert.NotNull(report);
        Assert.Equal(4, report.TotalTasksCompleted);
        Assert.Equal(2, report.DistinctUsersWhoCompletedTasks);
        Assert.Equal(2.0, report.AverageTasksCompletedPerUser);
    }

    /// <summary>
    /// Teste: relatório gerado não contém nenhuma eestatística das terefas concluídas, possui valores zerados.
    /// </summary>
    [Fact]
    public async Task GenerateAverageCompletedTasksReportAsync_ShouldReturnZeroAverage_WhenNoTasksExist()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId, Role = "Manager" };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock para retornar uma lista vazia.
        _taskRepositoryMock.Setup(repo => repo.GetCompletedTasksSinceAsync(It.IsAny<DateTime>()))
                           .ReturnsAsync(new List<TaskHistory>());

        // Act
        var report = await _reportService.GenerateAverageCompletedTasksReportAsync(userId);

        // Assert
        Assert.NotNull(report);
        Assert.Equal(0, report.TotalTasksCompleted);
        Assert.Equal(0, report.DistinctUsersWhoCompletedTasks);
        Assert.Equal(0.0, report.AverageTasksCompletedPerUser);
    }

    /// <summary>
    /// Teste: a exceção BusinessRuleException é lançada quando um usuário com Role diferente de "Manager" tenta acessar o relatório.
    /// </summary>
    [Fact]
    public async Task GenerateAverageCompletedTasksReportAsync_ShouldThrowBusinessRuleException_WhenRoleIsNotManager()
    {
        // Arrange
        var userId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId, Role = "User" };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock para retornar uma lista vazia.
        _taskRepositoryMock.Setup(repo => repo.GetCompletedTasksSinceAsync(It.IsAny<DateTime>()))
                           .ReturnsAsync(new List<TaskHistory>());

        // Act & Assert (Agir e Afirmar)
        // Verificamos se a chamada ao método GenerateAverageCompletedTasksReportAsync lança a exceção esperada.
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _reportService.GenerateAverageCompletedTasksReportAsync(userId)
        );

        // Verificar se a mensagem de erro é a correta.
        Assert.Equal("Você não tem permissão para acessar este relatório.", exception.Message);
    }
}