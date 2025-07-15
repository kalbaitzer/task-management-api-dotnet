using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.API.Controllers.Utils;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Interfaces;

namespace TaskManagementAPI.API.Controllers;

/// <summary>
/// Controller responsável por gerenciar os endpoints relacionados a Relatórios.
/// </summary>
[ApiController]
[Route("api/[controller]")]// Rota base: /api/reports
public class ReportsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IReportService _reportService;

    /// <summary>
    /// Construtor do Controller de Relatórios.
    /// </summary>
    /// <param name="userService">Serviço de usuários.</param>
    /// <param name="reportService">Serviço de relatórios.</param>
    public ReportsController(IUserService userService, IReportService reportService)
    {
        _userService = userService;
        _reportService = reportService;
    }

    /// <summary>
    /// Relatório de desempenho para as tarefas concluídas nos últimos 30 dias.
    /// </summary>
    /// <returns>Relatório de desempenho.</returns>
    [HttpGet("performance")]
    [ProducesResponseType(typeof(PerformanceReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPerformanceReport()
    {
        try
        {
            // Obtém o Id do usuário no cabeçalho do Request: X-User-Id
            var userId = ControllerHelper.GetUserId(Request);

            // Obtém o DTO referente ao relatório de desempenho
            var report = await _reportService.GenerateAverageCompletedTasksReportAsync(userId);

            if (report != null) return Ok(report);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }
}