using System.ComponentModel.DataAnnotations;
using TaskManagementAPI.Core.Entities;

namespace TaskManagementAPI.Application.DTOs;

public class CreateTaskDto
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public DateTime DueDate { get; set; }

    [Required]
    public TaskPriority Priority { get; set; }
}