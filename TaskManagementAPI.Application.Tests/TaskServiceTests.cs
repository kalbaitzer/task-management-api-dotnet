using Moq;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Application.Services;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using Task = System.Threading.Tasks.Task;
using TaskEntity = TaskManagementAPI.Core.Entities.Task;
using TaskStatus = TaskManagementAPI.Core.Entities.TaskStatus;

namespace TaskManagementAPI.Application.Tests;

public class TaskServiceTests
{
    private readonly Mock<ITaskRepository> _taskRepositoryMock;
    private readonly Mock<IProjectRepository> _projectRepositoryMock;
    private readonly Mock<ITaskHistoryRepository> _taskHistoryRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IUnitOfWork> _unitOfWorkMock;
    private readonly TaskService _taskService;

    // Construtor para inicializar os mocks e o serviço antes de cada teste
    public TaskServiceTests()
    {
        _taskRepositoryMock = new Mock<ITaskRepository>();
        _projectRepositoryMock = new Mock<IProjectRepository>();
        _taskHistoryRepositoryMock = new Mock<ITaskHistoryRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _unitOfWorkMock = new Mock<IUnitOfWork>();

        _taskService = new TaskService(
            _taskRepositoryMock.Object,
            _projectRepositoryMock.Object,
            _taskHistoryRepositoryMock.Object,
            _userRepositoryMock.Object,
            _unitOfWorkMock.Object
        );
    }

    // --- TESTES PARA O MÉTODO GetTaskByIdAsync ---

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnTaskDto_WhenTaskExists()
    {
        // Arrange (Arranjar)
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Cria a entidade de Projeto que "existe" no banco.
        var existingProject = new Project { Id = projectId };

        // Cria a entidade "falsa" que nosso repositório irá "encontrar".
        var task = new TaskEntity(
            "Tarefa de Teste",
            "Descrição para o teste",
            DateTime.UtcNow.AddDays(1),
            TaskPriority.Media,
            projectId
        );

        // Atribuímos o ID manualmente para o nosso controle no teste
        typeof(TaskEntity).GetProperty("Id")!.SetValue(task, taskId);

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositório para "encontrar" o projeto.
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

        // Configura o mock do repositório para retornar nossa entidade quando GetByIdAsync for chamado com este ID.
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                           .ReturnsAsync(task);

        // Act (Agir)
        // Executa o método que estamos testando.
        var result = await _taskService.GetTaskByIdAsync(taskId, userId);

        // Assert (Afirmar)
        // Verifica se o resultado não é nulo.
        Assert.NotNull(result);

        // Verifica se o resultado é do tipo correto (TaskDto).
        Assert.IsType<TaskDto>(result);

        // Verifica se os dados da entidade foram mapeados corretamente para o DTO.
        Assert.Equal(task.Id, result.Id);
        Assert.Equal(task.Title, result.Title);
        Assert.Equal(task.Priority, result.Priority);
    }

    [Fact]
    public async Task GetTaskByIdAsync_ShouldReturnNull_WhenTaskDoesNotExist()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Cria a entidade de Projeto que "existe" no banco.
        var existingProject = new Project { Id = projectId };

        // Configura o mock do repositório para retornar NULO.
        // Isso simula o cenário onde a tarefa não foi encontrada no banco de dados.
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId))
                           .ReturnsAsync((TaskEntity?)null); // O cast (TaskEntity) é para ajudar o compilador com o tipo genérico.

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositório para "encontrar" o projeto.
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

        // Act & Assert (Agir e Afirmar)
        // Verificamos se o método lança a exceção esperada (NotFoundException).
        // A ação (chamar GetTaskByIdAsync) é passada para o Assert.ThrowsAsync.
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.GetTaskByIdAsync(taskId, userId)
        );

        // Verifica se a mensagem da exceção é a correta.
        Assert.Equal("Tarefa não encontrada.", exception.Message);
    }

    // --- TESTE PARA A REGRA DE NEGÓCIO 1 ---

    [Fact]
    public async Task UpdateTaskDetailsAsync_ShouldNotChangePriority_WhenUpdatingOtherDetails()
    {
        // Arrange (Arranjar)
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var initialPriority = TaskPriority.Alta;

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Cria a entidade de Projeto que "existe" no banco.
        var existingProject = new Project { Id = projectId };

        // Cria a tarefa original que "existe" no banco de dados.
        var originalTask = new TaskEntity(
            "Título Original",
            "Descrição Original",
            DateTime.UtcNow.AddDays(5),
            initialPriority, // Prioridade inicial é Alta
            projectId
        );

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositórios para "encontrar" a tarefa.
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(originalTask);

        // Configura o mocks do repositórios para "encontrar" o projeto.
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

        // Cria o DTO com os dados da atualização. Note que ele não tem um campo de prioridade,
        // o que já é a nossa primeira linha de defesa.
        var updateDto = new UpdateTaskDto
        {
            Title = "Título Atualizado",
            Description = "Descrição Atualizada",
            DueDate = DateTime.UtcNow.AddDays(10),
            Status = TaskStatus.EmAndamento
        };

        // Act (Agir)
        // Executa o método de atualização que estamos a testar.
        await _taskService.UpdateTaskDetailsAsync(taskId, updateDto, userId);

        // Assert (Afirmar)
        // Verificação: Garantimos que a prioridade da entidade não mudou.
        Assert.Equal(initialPriority, originalTask.Priority);

        // Verificação: Garantimos que os outros campos FORAM atualizados.
        // Isso prova que o método de atualização realmente executou, mas respeitou a imutabilidade da prioridade.
        Assert.Equal("Título Atualizado", originalTask.Title);

        // Verificamos se o método para salvar as alterações foi chamado, confirmando que a transação foi completada.
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- TESTE PARA A REGRA DE NEGÓCIO 3 ---

    [Fact]
    public async Task UpdateTaskDetailsAsync_ShouldCreateHistoryRecord_WhenTitleIsChanged()
    {
        // Arrange (Arranjar)
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var projectId = Guid.NewGuid();
        var originalTitle = "Título Original da Tarefa";
        var newTitle = "Título da Tarefa Atualizado";

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Cria a entidade de Projeto que "existe" no banco.
        var existingProject = new Project { Id = projectId };

        var originalTask = new TaskEntity(
            originalTitle,
            "Descrição",
            DateTime.UtcNow,
            TaskPriority.Media,
            projectId
        );

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositórios para "encontrar" a tarefa.
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(originalTask);

        // Configura o mocks do repositórios para "encontrar" o projeto.
        _projectRepositoryMock.Setup(repo => repo.GetByIdAsync(projectId)).ReturnsAsync(existingProject);

        // DTO com o título alterado.
        var updateDto = new UpdateTaskDto
        {
            Title = newTitle,
            Description = originalTask.Description, // Descrição não muda neste teste
            DueDate = originalTask.DueDate,         // Data não muda
            Status = originalTask.Status            // Status não muda
        };

        // Act (Agir)
        // Executa o método que deve gerar o histórico.
        await _taskService.UpdateTaskDetailsAsync(taskId, updateDto, userId);

        // Assert (Afirmar)
        // AQUI ESTÁ A VERIFICAÇÃO PRINCIPAL:
        // Verificamos se o método AddAsync do repositório de histórico foi chamado
        // exatamente uma vez, com um objeto TaskHistory que corresponde às nossas expectativas.
        _taskHistoryRepositoryMock.Verify(
            repo => repo.AddAsync(It.Is<TaskHistory>(history =>
                history.TaskId == taskId &&
                history.UserId == userId &&
                history.ChangeType == "Update" &&
                history.FieldName == "Title" &&
                history.OldValue == originalTitle &&
                history.NewValue == newTitle
            )),
            Times.Once // Garante que foi chamado apenas uma vez.
        );

        // Também é uma boa prática garantir que a operação de salvar foi chamada.
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    // --- TESTE PARA A REGRA DE NEGÓCIO 4 ---

    [Fact]
    public async Task CreateTaskAsync_ShouldThrowBusinessRuleException_WhenProjectTaskLimitIsReached()
    {
        // Arrange (Arranjar)
        var projectId = Guid.NewGuid();
        var userId = Guid.NewGuid();

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Cria uma lista com 20 tarefas "falsas". Não precisamos preencher os detalhes delas,
        // apenas que existam 20 na lista.
        var taskList = new List<TaskEntity>();

        for (int i = 0; i < 20; i++)
        {
            taskList.Add(new TaskEntity("Tarefa " + i, "", DateTime.Now, TaskPriority.Baixa, projectId));
        }

        // Cria o projeto que já está "cheio".
        var fullProject = new Project
        {
            Id = projectId,
            Name = "Projeto Cheio",
            Tasks = taskList // A coleção de tarefas tem 20 itens.
        };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock do repositório de PROJETOS para retornar nosso projeto cheio.
        // O método CreateTaskAsync chama GetByIdWithTasksAsync para verificar o limite.
        _projectRepositoryMock.Setup(repo => repo.GetByIdWithTasksAsync(projectId))
                              .ReturnsAsync(fullProject);

        // Cria o DTO para a 21ª tarefa que estamos tentando adicionar.
        var dtoForNewTask = new CreateTaskDto { Title = "A 21ª Tarefa" };

        // Act & Assert (Agir e Afirmar)
        // Verificamos se a chamada ao método CreateTaskAsync lança a exceção esperada.
        var exception = await Assert.ThrowsAsync<BusinessRuleException>(
            () => _taskService.CreateTaskAsync(projectId, dtoForNewTask, userId)
        );

        // Opcional, mas recomendado: verificar se a mensagem de erro é a correta.
        Assert.Equal("Limite de 20 tarefas por projeto foi atingido.", exception.Message);

        // Verificação extra: Garantimos que o serviço NÃO tentou adicionar a nova tarefa
        // ou salvar as alterações, já que a regra de negócio falhou antes.
        _taskRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<TaskEntity>()), Times.Never);
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }
    
    // --- TESTE PARA A REGRA DE NEGÓCIO 6 ---

    [Fact]
    public async Task AddCommentAsync_ShouldCreateCommentHistoryRecord_WhenTaskExists()
    {
        // Arrange (Arranjar)
        var taskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comentario = "Este é um comentário de teste importante.";

        // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // DTO que seria enviado pela API.
        var commentDto = new AddCommentDto { Comment = comentario };

        // Simula a existência da tarefa à qual o comentário será adicionado.
        var existingTask = new TaskEntity("Tarefa Existente", "", DateTime.Now, TaskPriority.Baixa, Guid.NewGuid());

        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(taskId)).ReturnsAsync(existingTask);

        // Act (Agir)
        // Executa o método que estamos testando.
        await _taskService.AddCommentAsync(taskId, commentDto, userId);

        // Assert (Afirmar)
        // Verificamos se o método AddAsync do repositório de histórico foi chamado
        // exatamente uma vez com um objeto TaskHistory que representa nosso comentário.
        _taskHistoryRepositoryMock.Verify(
            repo => repo.AddAsync(It.Is<TaskHistory>(history =>
                history.TaskId == taskId &&
                history.UserId == userId &&
                history.ChangeType == "Comment" && // Verifica se o tipo da alteração está correto.
                history.Comment == comentario &&   // Verifica se o texto do comentário está correto.
                history.FieldName == null &&       // Garante que não é uma atualização de campo.
                history.OldValue == null &&
                history.NewValue == null
            )), 
            Times.Once // Garante que foi chamado apenas uma vez.
        );

        // Verificamos também se a operação foi salva no banco de dados.
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task AddCommentAsync_ShouldThrowNotFoundException_WhenTaskDoesNotExist()
    {
        // Arrange
        var nonExistentTaskId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var commentDto = new AddCommentDto { Comment = "Teste" };

         // Cria a entidade de Usuário que "existe" no banco.
        var user = new User { Id = userId };

        // Configura o mock do repositório para "encontrar" o usuário.
        _userRepositoryMock.Setup(repo => repo.GetByIdAsync(userId)).ReturnsAsync(user);

        // Configura o mock para simular que a tarefa não foi encontrada.
        _taskRepositoryMock.Setup(repo => repo.GetByIdAsync(nonExistentTaskId)).ReturnsAsync((TaskEntity?)null);

        // Act & Assert
        // Garante que uma exceção NotFoundException é lançada.
        var exception = await Assert.ThrowsAsync<NotFoundException>(
            () => _taskService.AddCommentAsync(nonExistentTaskId, commentDto, userId)
        );
        
 
        // Verifica se a mensagem da exceção é a correta.
        Assert.Equal("Tarefa não encontrada.", exception.Message);

        // Garante que, se a tarefa não existe, nada é salvo.
        _unitOfWorkMock.Verify(uow => uow.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
    }    
}