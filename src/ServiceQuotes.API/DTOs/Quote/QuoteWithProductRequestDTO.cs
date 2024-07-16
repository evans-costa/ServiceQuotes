using ServiceQuotes.API.Validations;

namespace ServiceQuotes.API.DTOs.Quote;

public class QuoteWithProductRequestDTO
{
    public QuoteRequestDTO? Quote { get; set; }
    [MinimumItemList]
    public List<QuoteProductsRequestDTO>? Products { get; set; }
}
