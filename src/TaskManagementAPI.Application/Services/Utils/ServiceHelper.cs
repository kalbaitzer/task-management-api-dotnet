using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Exceptions;
using TaskManagementAPI.Application.Interfaces.Repositories;
using TaskManagementAPI.Core.Entities;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using TaskEntity = TaskManagementAPI.Core.Entities.Task;

namespace TaskManagementAPI.Application.Services.Utils;

/// <summary>
/// Classe Utilitária para validações nos serviços
/// </summary>
public static class ServiceHelper
{
    /// <summary>
    /// Verifica se o ID do usuário é válido e se está cadastrado.
    /// </summary>
    /// <param name="userId">ID do usuário.</param>
    /// <param name="userRepository">Repositório para usuários.</param>
    /// <returns>A Entidade User.</returns>
    public static async Task<User?> CheckUser(Guid userId, IUserRepository userRepository)
    {
        if (userId == Guid.Empty)
        {
            // Retorna um erro se o cabeçalho essencial não for fornecido.
            throw new NotFoundException("Usuário ausente ou inválido.");
        }

        var user = await userRepository.GetByIdAsync(userId);

        if (user == null)
        {
            // Retorna um erro se o usuário não estiver cadastrado.
            throw new NotFoundException("Usuário não cadastrado.");
        }

        return user;
    }

    /// <summary>
    /// Verifica se o projeto está cadastrado.
    /// </summary>
    /// <param name="projectId">ID do projeto.</param>
    /// <param name="projectRepository">Repositório para projetos.</param>
    /// <returns>A Entidade Project.</returns>
    public static async Task<Project?> CheckProject(Guid projectId, IProjectRepository projectRepository)
    {
        // Valida se o projeto existe
        var project = await projectRepository.GetByIdWithTasksAsync(projectId);

        if (project == null)
        {
            // Lança uma exceção de projeto não encontrado
            throw new NotFoundException("Projeto não encontrado.");
        }

        return project;
    }

    /// <summary>
    /// Verifica se a tarefa está cadastrada.
    /// </summary>
    /// <param name="taskId">ID da tarefa.</param>
    /// <param name="taskRepository">Repositório para tarefas.</param>
    /// <returns>A Entidade TaskEntity.</returns>
    public static async Task<TaskEntity?> CheckTask(Guid taskId, ITaskRepository taskRepository)
    {
        var task = await taskRepository.GetByIdAsync(taskId);

        if (task == null)
        {
            // Lança uma exceção de tarefa não encontrada
            throw new NotFoundException("Tarefa não encontrada.");
        }

        return task;
    }

    /// <summary>
    /// Verifica se o usuário está cadastrado e se é um gerente (Manager).
    /// </summary>
    /// <param name="userId">ID do projeto.</param>
    /// <param name="userRepository">Repositório para usuários.</param>
    /// <returns>A Entidade Project.</returns>
    public static async Task<User?> CheckManager(Guid userId, IUserRepository userRepository)
    {
        // Verificação do usuário
        var user = await CheckUser(userId, userRepository);

        if (user != null)
        {
            if (user.Role != "Manager")
            {
                // Lança uma exceção de acesso negado
                throw new ForbiddenAccessException("Você não tem permissão para acessar este relatório.");
            }
        }

        return user;
    }
}