using QuestPDF.Fluent;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Helpers;
using ServiceQuotes.Models;

namespace ServiceQuotes.Services;

public class InvoiceService : IInvoiceService
{
    public byte[] GenerateInvoiceDocument(QuoteDetailedResponseDTO quote)
    {
        var invoice = new Invoice
        {
            InvoiceNumber = quote.QuoteId,
            CreatedAt = quote.CreatedAt,

            CustomerInfo = new CustomerInfo
            {
                Name = quote.CustomerInfo?.Name,
                Phone = quote.CustomerInfo?.Phone
            },

            Items = quote.Products?.Select(product => new OrderItem
            {
                Name = product.Name,
                Price = product.Price,
                Quantity = product.Quantity,
            }).ToList()
        };

        var document = new InvoiceDocumentTemplate(invoice);
        return document.GeneratePdf();
    }
}
