using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.Application.DTO.Quote;

public class QuoteRequestDTO
{
    [Required]
    public Guid CustomerId { get; set; }
}
