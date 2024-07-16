using System.Net;

namespace ServiceQuotes.API.Exceptions;

[Serializable]
public abstract class ServiceQuoteException(string message) : Exception(message)
{
    public abstract HttpStatusCode GetStatusCode();
    public abstract IList<string> GetErrorMessages();
}
