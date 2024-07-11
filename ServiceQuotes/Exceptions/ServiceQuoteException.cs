using System.Net;

namespace ServiceQuotes.Exceptions;

[Serializable]
public abstract class ServiceQuoteException(string message) : Exception(message)
{
    public abstract HttpStatusCode GetStatusCode();
    public abstract IList<string> GetErrorMessages();
}
