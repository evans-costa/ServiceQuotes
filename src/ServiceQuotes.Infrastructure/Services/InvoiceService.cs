using QuestPDF.Fluent;
using ServiceQuotes.Application.DTO.Invoice;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Infrastructure.Helpers;
using System.Globalization;

namespace ServiceQuotes.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IS3BucketService _bucketService;

    public InvoiceService(IS3BucketService bucketService)
    {
        _bucketService = bucketService;
    }

    public byte[] GenerateInvoiceDocument(Quote quote)
    {
        var invoice = new InvoiceDTO
        {
            InvoiceNumber = quote.QuoteId,
            CreatedAt = quote.CreatedAt,

            CustomerInfo = new CustomerInfo
            {
                Name = quote.Customer?.Name,
                Phone = quote.Customer?.Phone
            },

            Items = quote.QuotesProducts?.Select(product => new OrderItem
            {
                Name = quote.Products.FirstOrDefault(p => p.ProductId == product.ProductId)?.Name,
                Price = product.Price,
                Quantity = product.Quantity,
            }).ToList()
        };

        var culture = CultureInfo.CreateSpecificCulture("pt-BR");
        var document = new InvoiceDocumentTemplate(invoice, culture);

        return document.GeneratePdf();
    }

    public async Task<string> GenerateInvoiceUrl(Quote quote)
    {
        var invoiceDocument = GenerateInvoiceDocument(quote);

        var fileName = $"invoice_{quote.CreatedAt:yyyyMMddTHHmmss}_{quote.QuoteId:d8}.pdf";

        var fileUrl = await _bucketService.UploadFileToS3(invoiceDocument, fileName);

        return fileUrl;
    }
}
