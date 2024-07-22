using QuestPDF.Fluent;
using ServiceQuotes.Application.DTO.Invoice;
using ServiceQuotes.Application.DTO.Quote;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Infrastructure.Helpers;
using System.Globalization;

namespace ServiceQuotes.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    public byte[] GenerateInvoiceDocument(QuoteDetailedResponseDTO quote)
    {
        var invoice = new InvoiceDTO
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

        var culture = CultureInfo.CreateSpecificCulture("pt-BR");
        var document = new InvoiceDocumentTemplate(invoice, culture);

        return document.GeneratePdf();
    }
}
