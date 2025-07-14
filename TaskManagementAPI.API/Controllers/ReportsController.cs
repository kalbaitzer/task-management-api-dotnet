using Microsoft.AspNetCore.Mvc;
using TaskManagementAPI.API.Controllers.Utils;
using TaskManagementAPI.Application.DTOs;
using TaskManagementAPI.Application.Interfaces;

namespace TaskManagementAPI.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportsController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IReportService _reportService;

    public ReportsController(IUserService userService, IReportService reportService)
    {
        _userService = userService;
        _reportService = reportService;
    }

    [HttpGet("performance")]
    [ProducesResponseType(typeof(PerformanceReportDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> GetPerformanceReport()
    {
        try
        {
            // Verificação de permissão, se é gerente
            await ControllerHelper.CheckManager(Request,_userService);

            var report = await _reportService.GenerateAverageCompletedTasksReportAsync();

            if (report != null) return Ok(report);
        }
        catch (Exception e)
        {
            return StatusCode(ControllerHelper.GetStatusCode(e),new { error = e.Message });
        }

        return NoContent();
    }
}