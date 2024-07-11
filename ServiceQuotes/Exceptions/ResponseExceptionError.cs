namespace ServiceQuotes.Exceptions;

public class ResponseExceptionError
{
    public IList<string> Errors { get; set; } = [];

    public ResponseExceptionError(IList<string> errors)
    {
        Errors = errors;
    }
}
