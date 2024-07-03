using ServiceQuotes.DTOs.ResponseDTO;

namespace ServiceQuotes.Services;

public interface IInvoiceService
{
    byte[] GenerateInvoiceDocument(QuoteDetailedResponseDTO quote);
}
