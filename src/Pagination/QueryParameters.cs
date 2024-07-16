namespace ServiceQuotes.Pagination;

public class QueryParameters
{
    const int maxPageSize = 50;
    private int _pageNumber = 1;
    private int _pageSize = 10;

    public int PageNumber
    {
        get { return _pageNumber; }
        set { _pageNumber = value > 0 ? value : 1; }
    }
    public int PageSize
    {
        get { return _pageSize; }
        set { _pageSize = value > maxPageSize ? maxPageSize : value; }
    }
}
