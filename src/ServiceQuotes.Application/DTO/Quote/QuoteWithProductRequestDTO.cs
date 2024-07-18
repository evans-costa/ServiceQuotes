using ServiceQuotes.Application.Validations;

namespace ServiceQuotes.Application.DTO.Quote;

public class QuoteWithProductRequestDTO
{
    public QuoteRequestDTO? Quote { get; set; }
    [MinimumItemList]
    public List<QuoteProductsRequestDTO>? Products { get; set; }
}
