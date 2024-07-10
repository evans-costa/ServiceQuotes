using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.Customer;

public class CustomerRequestDTO
{
    [Required(ErrorMessage = "O nome é obrigatório.")]
    [StringLength(80, MinimumLength = 3)]
    public string? Name { get; set; }
    public string? Phone { get; set; }
}