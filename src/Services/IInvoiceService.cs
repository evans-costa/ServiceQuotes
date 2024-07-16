using ServiceQuotes.DTOs.Quote;

namespace ServiceQuotes.Services;

public interface IInvoiceService
{
    byte[] GenerateInvoiceDocument(QuoteDetailedResponseDTO quote);
}
