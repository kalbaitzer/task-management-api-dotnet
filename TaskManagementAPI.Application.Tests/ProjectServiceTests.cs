using Moq;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;

namespace TaskManagementAPI.Application.Tests;

public class ProjectServiceTests
{
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly ProjectService _projectService;

    // O construtor da classe de teste é executado antes de cada teste,
    // garantindo um ambiente limpo.
    public ProjectServiceTests()
    {
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        // Instancia o serviço que queremos testar, injetando os mocks.
        _projectService = new ProjectService(
            _projectRepositoryMock.Object,
            _taskRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object);
    }

    // --- TESTES PARA O MÉTODO GetProjectByIdAsync ---

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturnProjectDetailDto_WhenProjectExists()
    {
        // Arrange (Arranjar)
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Cria a entidade de Projeto que "existe" no banco.
        var project = new Project
        {
            Id = projectId,
            Name = "Projeto de Teste",
            Description = "Descrição para o teste", 
            CreatedAt = DateTime.UtcNow
        };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositório para "encontrar" o projeto.
        _projectRepositoryMock.Setup(repo => repo.GetByIdWithTasksAsync(projectId)).ReturnsAsync(project);

        // Act (Agir)
        // Executa o método que estamos testando.
        var result = await _projectService.GetProjectByIdAsync(projectId, userId);

        // Assert (Afirmar)
        // Verifica se o resultado não é nulo.
        Assert.NotNull(result);
        
        // Verifica se o resultado é do tipo correto (ProjectDetailsDto).
        Assert.IsType<ProjectDetailDto>(result);
        
        // Verifica se os dados da entidade foram mapeados corretamente para o DTO.
        Assert.Equal(project.Id, result.Id);
        Assert.Equal(project.Name, result.Name);
        Assert.Equal(project.Description, result.Description);
        Assert.Equal(project.CreatedAt, result.CreatedAt);
    }

    [Fact]
    public async Task GetProjectByIdAsync_ShouldReturnNull_WhenProjectDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositório para retornar NULO.
        // Isso simula o cenário onde a tarefa não foi encontrada no banco de dados.
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId))
                               .ReturnsAsync((Project?) null); // O cast (Project) é para ajudar o compilador com o tipo genérico.

        // Act & Assert (Agir e Afirmar)
        // Verificamos se o método lança a exceção esperada (NotFoundException).
        // A ação (chamar GetProjectByIdAsync) é passada para o Assert.ThrowsAsync.
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _projectService.GetProjectByIdAsync(projectId, userId)
        );

        // Verifica se a mensagem da exceção é a correta.
        Assert.Equal("Projeto não encontrado.", exception.Message);
    }

    // --- TESTE PARA A REGRA DE NEGÓCIO 2 ---

    [Fact]
    public async Task DeleteProjectAsync_ShouldThrowBusinessRuleException_WhenProjectHasActiveTasks()
    {
        // Arrange (Arranjar)
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = new Project { Id = projectId, OwnerUserId = userId };

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositório de projeto para retornar o projeto quando for buscado.
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId))
                              .ReturnsAsync(project);

        // Dizemos ao mock do repositório de tarefas para retornar 'true' quando perguntado se há tarefas ativas.
        _taskRepositoryMock.Setup(repo => repo.HasActiveTasksInProjectAsync(projectId))
                           .ReturnsAsync(true);

        // Act & Assert (Agir e Afirmar)
        // Verificamos se o método lança a exceção esperada (BusinessRuleException).
        // A ação (chamar DeleteProjectAsync) é passada para o Assert.ThrowsAsync.
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _projectService.DeleteProjectAsync(projectId, userId)
        );

        // Verifica se a mensagem da exceção é a correta.
        Assert.Equal("Não é possível remover o projeto. Existem tarefas pendentes ou em andamento. Conclua ou remova as tarefas primeiro.", exception.Message);
    }

    [Fact]
    public async Task DeleteProjectAsync_ShouldCallDeleteAndSaveChanges_WhenProjectHasNoActiveTasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var project = new Project { Id = projectId, OwnerUserId = userId };

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId))
                              .ReturnsAsync(project);

        // Desta vez, configuramos o mock para retornar 'false' (não há tarefas ativas).
        _taskRepositoryMock.Setup(repo => repo.HasActiveTasksInProjectAsync(projectId))
                           .ReturnsAsync(false);

        // Act
        // Executamos o método. Não esperamos nenhuma exceção.
        await _projectService.DeleteProjectAsync(projectId, userId);

        // Assert
        // Verificamos se os métodos corretos foram chamados nos nossos mocks.
        // Queremos garantir que o projeto foi de fato marcado para deleção...
        _projectRepositoryMock.Verify(repo => repo.Delete(project), Times.Once);

        // ...e que as alterações foram salvas.
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}