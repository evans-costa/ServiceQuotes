using ServiceQuotes.Validations;
using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.DTOs.Quote;

public class QuoteWithProductRequestDTO
{
    [Required]
    public QuoteRequestDTO? Quote { get; set; }
    [Required]
    [MinimumItemList]
    public List<QuoteProductsRequestDTO>? Products { get; set; }
}
