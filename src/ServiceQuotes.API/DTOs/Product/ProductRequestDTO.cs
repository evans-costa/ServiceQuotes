using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.API.DTOs.Product;

public class ProductRequestDTO
{
    [Required(ErrorMessage = "O nome é obrigatório")]
    [StringLength(200, MinimumLength = 5, ErrorMessage = "O nome deve ter mais que 5 caracteres")]
    public string? Name { get; set; }
}
