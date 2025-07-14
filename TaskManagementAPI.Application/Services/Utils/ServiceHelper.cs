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

    public static async Task<User?> CheckManager(Guid userId, IUserRepository userRepository)
    {
        // Verificação do usuário
        var user = await CheckUser(userId, userRepository);

        if (user != null)
        {
            if (user.Role != "Manager")
            {
                // Retorna HTTP 403 Forbidden
                throw new ForbiddenAccessException("Você não tem permissão para acessar este relatório.");
            }
        }

        return user;
    }
}