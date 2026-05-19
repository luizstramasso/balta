using System.ComponentModel.DataAnnotations;

namespace Fina.Core.Requests.Categories;

public class UpdateCategoryRequest : Request
{
    public long Id { get; set; }
    [Required(ErrorMessage = "Título inválido.")]
    [MaxLength(100, ErrorMessage = "O título deve conter no máximo 100 caracteres.")]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Descrição inválida.")]
    public string? Description { get; set; }
}
