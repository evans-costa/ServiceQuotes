using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.API.DTOs.Quote;

public class QuoteRequestDTO
{
    [Required]
    public Guid CustomerId { get; set; }
}
