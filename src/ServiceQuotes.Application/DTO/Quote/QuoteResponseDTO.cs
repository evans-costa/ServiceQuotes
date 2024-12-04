using ServiceQuotes.Application.DTO.Customer;

namespace ServiceQuotes.Application.DTO.Quote;

public class QuoteResponseDTO
{
    public int QuoteId { get; set; }
    public CustomerResponseDTO? CustomerInfo { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
}
