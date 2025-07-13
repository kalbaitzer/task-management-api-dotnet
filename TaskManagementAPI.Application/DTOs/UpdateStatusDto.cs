using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Core.Entities; // Para usar o enum TaskStatus

namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO específico para a atualização do status de uma tarefa.
/// Usar um DTO tão granular previne a alteração acidental de outros campos
/// e permite a criação de endpoints mais específicos e RESTful (ex: usando o verbo PATCH).
/// </summary>
public class UpdateStatusDto
{
    /// <summary>
    /// O novo status para a tarefa.
    /// A validação [Required] garante que o cliente sempre envie um valor.
    /// </summary>
    [Required(ErrorMessage = "O status é obrigatório.")]
    public TaskManagementAPI.Core.Entities.TaskStatus Status { get; set; }
}