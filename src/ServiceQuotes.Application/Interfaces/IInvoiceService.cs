using ServiceQuotes.Domain.Entities;

namespace ServiceQuotes.Application.Interfaces;

public interface IInvoiceService
{
    byte[] GenerateInvoiceDocument(Quote quote);
    Task<string> GenerateInvoiceUrl(Quote quote);
}
