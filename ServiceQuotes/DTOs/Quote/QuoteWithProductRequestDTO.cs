using ServiceQuotes.Validations;

namespace ServiceQuotes.DTOs.Quote;

public class QuoteWithProductRequestDTO
{
    public QuoteRequestDTO? Quote { get; set; }
    [MinimumItemList]
    public List<QuoteProductsRequestDTO>? Products { get; set; }
}
