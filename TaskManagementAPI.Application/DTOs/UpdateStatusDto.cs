using System.ComponentModel.DataAnnotations;

// Mapeamentos para evitar conflitos entre classes com mesmo nome
using TaskStatus = TaskManagementAPI.Core.Entities.TaskStatus;

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
    public TaskStatus Status { get; set; }
}