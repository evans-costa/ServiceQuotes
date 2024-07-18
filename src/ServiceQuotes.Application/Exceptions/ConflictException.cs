using System.Net;

namespace ServiceQuotes.Application.Exceptions;

[Serializable]
public class ConflictException(string message) : ServiceQuoteException(message)
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
        return HttpStatusCode.Conflict;
    }
}
