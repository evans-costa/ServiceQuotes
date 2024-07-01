using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.RequestDTO;

public class AddProductsToQuoteRequestDTO
{
    [Required]
    public int QuoteId { get; set; }
    [Required]
    public List<QuoteProductsRequestDTO>? Products { get; set; }
}
