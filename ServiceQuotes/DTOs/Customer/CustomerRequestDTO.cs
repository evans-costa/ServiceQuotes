using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.Customer;

public class CustomerRequestDTO
{
    [Required]
    [StringLength(80, MinimumLength = 3)]
    public string? Name { get; set; }
    public string? Phone { get; set; }
}
