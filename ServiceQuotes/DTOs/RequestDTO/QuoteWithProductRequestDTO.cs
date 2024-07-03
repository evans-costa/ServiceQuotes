using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.RequestDTO;

public class QuoteWithProductRequestDTO
{
    [Required]
    public QuoteRequestDTO? Quote { get; set; }
    public List<QuoteProductsRequestDTO>? Products { get; set; }
}
