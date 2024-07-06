using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.Product;

public class ProductRequestDTO
{
    [Required]
    [StringLength(200, MinimumLength = 5)]
    public string? Name { get; set; }
}
