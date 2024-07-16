using System.Net;

namespace ServiceQuotes.Exceptions;

[Serializable]
public class NotFoundException(string message) : ServiceQuoteException(message)
{
    public override IList<string> GetErrorMessages()
    {
        return
        [
            Message
        ];
    }

    public override HttpStatusCode GetStatusCode()
    {
        return HttpStatusCode.NotFound;
    }
}
