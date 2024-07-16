using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.Quote;

public class QuoteRequestDTO
{
    [Required]
    public Guid CustomerId { get; set; }
}
