using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.API.Controllers.Utils;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Interfaces;

namespace TaskManagementAPI.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os endpoints relacionados a Projetos.
/// </summary>
[ApiController]
[Route("api/[controller]")] // Rota base: /api/projects
public class ProjectsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IProjectService _projectService;

    /// <summary>
    /// Construtor do Controller de Projetos.
    /// </summary>
    /// <param name="userService">Serviço de usuários.</param>
    /// <param name="projectService">Serviço de projetos.</param>
    public ProjectsController(IUserService userService, IProjectService projectService)
    {
        _userService = userService;
        _projectService = projectService;
    }

    /// <summary>
    /// Cria um novo projeto para o usuário autenticado.
    /// </summary>
    /// <param name="projectDto">Os dados do novo projeto.</param>
    /// <returns>Os detalhes do projeto recém-criado.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
     public async Task<IActionResult> CreateProject([FromBody] CreateProjectDto projectDto)
    {
        try
        {
            // Obtém o Id do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Cria o novo projeto
            var newProject = await _projectService.CreateProjectAsync(projectDto, userId);

            if (newProject != null)
            {
                // Retorna o novo projeto criado
                return CreatedAtAction(nameof(GetProjectById), new { projectId = newProject.Id }, newProject);
            }
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Lista todos os projetos pertencentes ao usuário autenticado.
    /// </summary>
    /// <returns>Uma lista de projetos.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ProjectDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserProjects()
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Obtém a lista de projetos cadastrados
            var projects = await _projectService.GetUserProjectsAsync(userId);

            if (projects != null) return Ok(projects);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }

    /// <summary>
    /// Busca um projeto específico pelo seu ID.
    /// </summary>
    /// <param name="projectId">O ID do projeto a ser buscado.</param>
    /// <returns>Os detalhes do projeto.</returns>
    [HttpGet("{projectId}")]
    [ProducesResponseType(typeof(ProjectDetailDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetProjectById(Guid projectId)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Obtém o projeto cadastrado pelo seu ID
            var project = await _projectService.GetProjectByIdAsync(projectId, userId);

            if (project != null) return Ok(project);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NotFound(); 
    }

    /// <summary>
    /// Remove um projeto específico pelo seu ID.
    /// </summary>
    /// <param name="projectId">O ID do projeto a ser removido.</param>
    /// <returns>Nenhum conteúdo.</returns>
    [HttpDelete("{projectId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteProject(Guid projectId)
    {
        try
        {
            // Obtém o ID do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Remove o projeto pelo seu ID
            await _projectService.DeleteProjectAsync(projectId, userId);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        // HTTP 204 No Content é a resposta padrão para uma deleção bem-sucedida.
        return NoContent();
    }
}