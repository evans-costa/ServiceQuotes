namespace ServiceQuotes.Domain.Pagination;

public class QuoteFilterParams : QueryParameters
{
    public FilterCriteria? FilterCriteria { get; set; }
    public string? CustomerName { get; set; }
    public string? CreatedDate { get; set; }
}

public enum FilterCriteria
{
    Date,
    Customer
}
