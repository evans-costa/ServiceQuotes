namespace ServiceQuotes.API.Pagination;

public class QuoteFilterParams : QueryParameters
{
    public string? FilterCriteria { get; set; }
    public string? CustomerName { get; set; }
    public string? CreatedDate { get; set; }
}
