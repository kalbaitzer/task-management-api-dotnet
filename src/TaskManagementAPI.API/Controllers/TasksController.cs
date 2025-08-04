using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.API.Controllers.Utils;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Interfaces;
using TaskManagementAPI.Application.Interfaces.Repositories;

namespace TaskManagementAPI.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os endpoints relacionados a Tarefas.
/// </summary>
[ApiController]
[Route("api/[controller]")] // Rota base: /api/tasks
public class TasksController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;

    /// <summary>
    /// Construtor do Controller de Tarefas.
    /// </summary>
    /// <param name="userService">Serviço de usuários.</param>
    /// <param name="taskService">Serviço de tarefas.</param>
    public TasksController(IUserService userService, ITaskService taskService)
    {
        _userService = userService;
        _taskService = taskService;
    }

    /// <summary>
    /// Cria uma nova tarefa dentro de um projeto específico.
    /// </summary>
    /// <param name="projectId">O ID do projeto onde a tarefa será criada.</param>
    /// <param name="createTaskDto">Os dados da nova tarefa.</param>
    /// <returns>Os detalhes do usuário recém-criado.</returns>
    [HttpPost("projects/{projectId:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CreateTaskForProject(Guid projectId, [FromBody] CreateTaskDto createTaskDto)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Cria uma nova tarefa
            var newTask = await _taskService.CreateTaskAsync(projectId, createTaskDto, userId);

            if (newTask != null)
            {
                // Retorna a nova tarefa criada
                return CreatedAtAction(nameof(GetTaskById), new { taskId = newTask.Id }, newTask);
            }
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Lista todas as tarefas de um projeto específico.
    /// </summary>
    /// <param name="projectId">O ID do projeto.</param>
    /// <returns>Uma lista de tarefas do projeto.</returns>
    [HttpGet("projects/{projectId:guid}")]
    [ProducesResponseType(typeof(IEnumerable<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTasksByProject(Guid projectId)
    {
        try
        {
            // Obtém o Id do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Obtém a lista de tarefas cadastradas pelo ID do projeto
            var tasks = await _taskService.GetTasksByProjectAsync(projectId, userId);

            if (tasks != null) return Ok(tasks);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NotFound();
    }

    /// <summary>
    /// Busca uma tarefa específica pelo seu ID.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <returns>Os detalhes da tarefa.</returns>
    [HttpGet("{taskId:guid}")]
    [ProducesResponseType(typeof(TaskDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTaskById(Guid taskId)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Obtém a tarefa cadastrada pelo seu ID
            var task = await _taskService.GetTaskByIdAsync(taskId, userId);

            if (task != null) return Ok(task);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }
         
        return NotFound();
    }

    /// <summary>
    /// Atualiza os detalhes de uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="updateTaskDto">Os novos dados da tarefa.</param>
    /// <returns>Nenhum conteúdo.</returns>
    [HttpPut("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTask(Guid taskId, [FromBody] UpdateTaskDto updateTaskDto)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Atualiza os detalhes da tarefa através dados do DTO
            await _taskService.UpdateTaskDetailsAsync(taskId, updateTaskDto, userId);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Atualiza o status de uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="statusDto">O novo status da tarefa.</param>
    /// <returns>Nenhum conteúdo.</returns>
    [HttpPatch("{taskId:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> UpdateTaskStatus(Guid taskId, [FromBody] UpdateStatusDto statusDto)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Atualiza o status da tarefa
            await _taskService.UpdateTaskStatusAsync(taskId, statusDto, userId);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Adiciona um comentário a uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    /// <param name="commentDto">O comentário a ser adicionado.</param>
    /// <returns>Status Code 201.</returns>
    [HttpPost("{taskId:guid}/comments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> AddComment(Guid taskId, [FromBody] AddCommentDto commentDto)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Atualiza o status da tarefa
            await _taskService.AddCommentAsync(taskId, commentDto, userId);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return StatusCode(201);
    }

    /// <summary>
    /// Remove uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa a ser removida.</param>
    /// <returns>Nenhum conteúdo.</returns>
    [HttpDelete("{taskId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteTask(Guid taskId)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Remove a tarefa pelo seu ID
            await _taskService.DeleteTaskAsync(taskId,userId);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        // HTTP 204 No Content é a resposta padrão para uma deleção bem-sucedida.
        return NoContent();
    }

    /// <summary>
    /// Busca o histórico completo de uma tarefa.
    /// </summary>
    /// <param name="taskId">O ID da tarefa.</param>
    [HttpGet("{taskId:guid}/history")] // Rota direta para sub-recurso
    [ProducesResponseType(typeof(IEnumerable<Core.Entities.TaskHistory>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetTaskHistory(Guid taskId)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Obtém o histórico da tarefa pelo seu ID
            var history = await _taskService.GetTaskHistoryAsync(taskId, userId);

            if (history != null) return Ok(history);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NotFound();
    }
}