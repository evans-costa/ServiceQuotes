using ServiceQuotes.API.DTOs.Customer;
using ServiceQuotes.API.DTOs.Product;

namespace ServiceQuotes.API.DTOs.Quote;

public class QuoteDetailedResponseDTO
{
    public int QuoteId { get; set; }
    public CustomerResponseDTO? CustomerInfo { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public List<ProductDetailResponseDTO>? Products { get; set; }

}
