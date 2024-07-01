namespace ServiceQuotes.DTOs.ResponseDTO;

public class QuoteDetailedResponseDTO
{
    public int QuoteId { get; set; }
    public decimal TotalPrice { get; set; }
    public string? FileUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CustomerId { get; set; }
    public List<ProductDetailDTO>? Products { get; set; }

}
