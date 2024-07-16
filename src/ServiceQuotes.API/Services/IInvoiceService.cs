using ServiceQuotes.API.DTOs.Quote;

namespace ServiceQuotes.API.Services;

public interface IInvoiceService
{
    byte[] GenerateInvoiceDocument(QuoteDetailedResponseDTO quote);
}
