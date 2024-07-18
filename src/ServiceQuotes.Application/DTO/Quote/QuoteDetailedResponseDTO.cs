using ServiceQuotes.Application.DTO.Customer;
using ServiceQuotes.Application.DTO.Product;

namespace ServiceQuotes.Application.DTO.Quote;

public class QuoteDetailedResponseDTO
{
    public int QuoteId { get; set; }
    public CustomerResponseDTO? CustomerInfo { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProductDetailResponseDTO>? Products { get; set; }

}
