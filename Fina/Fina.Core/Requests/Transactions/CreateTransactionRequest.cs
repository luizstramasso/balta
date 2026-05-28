using System.ComponentModel.DataAnnotations;
using Fina.Core.Enums;

namespace Fina.Core.Requests.Transactions;

public class CreateTransactionRequest : Request
{
    [Required(ErrorMessage = "Título inválido.")]
    [MaxLength(80, ErrorMessage = "O título deve conter no máximo 80 caracteres.")]
    public string Title { get; set; } = null!;

    [Required(ErrorMessage = "Tipo inválido.")]
    public ETransactionType Type { get; set; }

    [Required(ErrorMessage = "Valor inválido.")]
    public decimal Amount { get; set; }

    [Required(ErrorMessage = "Categoria inválida.")]
    public long CategoryId { get; set; }

    [Required(ErrorMessage = "Data inválida.")]
    public DateTime? PaidOrReceivedAt { get; set; }
}
