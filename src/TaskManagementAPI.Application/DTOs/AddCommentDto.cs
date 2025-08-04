using System.ComponentModel.DataAnnotations;

namespace TaskManagementAPI.Application.DTOs;

/// <summary>
/// DTO para receber o conteúdo de um novo comentário a ser adicionado a uma tarefa.
/// </summary>
public class AddCommentDto
{
    /// <summary>
    /// O texto do comentário.
    /// É obrigatório para evitar comentários vazios e tem um limite de tamanho
    /// para prevenir abuso e garantir a integridade dos dados.
    /// </summary>
    [Required(ErrorMessage = "O conteúdo do comentário não pode ser vazio.")]
    [MaxLength(1000, ErrorMessage = "O comentário não pode exceder 1000 caracteres.")]
    public string Comment { get; set; } = string.Empty;
}