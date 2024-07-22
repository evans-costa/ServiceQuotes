using AutoMapper;
using ServiceQuotes.Application.DTO.Quote;
using ServiceQuotes.Application.Exceptions;
using ServiceQuotes.Application.Exceptions.Resources;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Domain.Pagination;
using X.PagedList.Extensions;

namespace ServiceQuotes.Application.Services;
public class QuoteService : IQuoteService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IS3BucketService _bucketService;
    private readonly IInvoiceService _invoiceService;

    public QuoteService(IUnitOfWork unitOfWork, IMapper mapper, IS3BucketService bucketService, IInvoiceService invoiceService)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _bucketService = bucketService;
        _invoiceService = invoiceService;
    }

    public async Task<QuoteResponseDTO> CreateQuote(QuoteWithProductRequestDTO quoteWithProductsDto)
    {
        if (quoteWithProductsDto is null || quoteWithProductsDto is { Products: null } or { Quote: null })
            throw new BadRequestException(ExceptionMessages.EMPTY_FIELDS);

        var customerExists = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == quoteWithProductsDto.Quote.CustomerId);

        if (customerExists is null)
            throw new NotFoundException(ExceptionMessages.CUSTOMER_NOT_FOUND);

        var quote = _mapper.Map<Quote>(quoteWithProductsDto.Quote);

        foreach (var product in quoteWithProductsDto.Products)
        {
            var productExists = await _unitOfWork.ProductRepository.GetAsync(e => e.ProductId == product.ProductId);

            if (productExists is null)
                throw new NotFoundException(ExceptionMessages.PRODUCT_NOT_FOUND);

            if (quote.QuotesProducts.Any(qp => qp.ProductId == product.ProductId))
                throw new ConflictException(ExceptionMessages.PRODUCT_DUPLICATED);

            var quoteProduct = _mapper.Map<QuoteProducts>(product);

            quote.QuotesProducts.Add(quoteProduct);
        }

        quote.TotalPrice = quote.QuotesProducts.Sum(p => p.Price * p.Quantity);

        await _unitOfWork.QuotesRepository.CreateAsync(quote);

        await _unitOfWork.CommitAsync();

        await SaveInvoiceOnQuote(quote.QuoteId);

        return _mapper.Map<QuoteResponseDTO>(quote);
    }

    public async Task<(IEnumerable<QuoteResponseDTO>, object)> GetAllQuotes(QueryParameters quoteParams)
    {
        var quotesEntities = await _unitOfWork.QuotesRepository.GetQuotesAsync();

        if (quotesEntities is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_NOT_FOUND);

        var quotesPaginated = quotesEntities.ToPagedList(quoteParams.PageNumber, quoteParams.PageSize);

        var metadata = new
        {
            quotesPaginated.PageNumber,
            quotesPaginated.PageSize,
            quotesPaginated.PageCount,
            quotesPaginated.TotalItemCount,
            quotesPaginated.HasNextPage,
            quotesPaginated.HasPreviousPage,
        };

        var quoteDto = _mapper.Map<IEnumerable<QuoteResponseDTO>>(quotesPaginated);

        return (quoteDto, metadata);
    }

    public async Task<(IEnumerable<QuoteResponseDTO>, object)> GetQuoteBySearch(QuoteFilterParams quoteFilterParams)
    {
        var quotesEntities = await _unitOfWork.QuotesRepository.SearchQuotesAsync(quoteFilterParams);

        if (quotesEntities is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_SEARCH_NOT_FOUND);

        var quotesPaginated = quotesEntities.ToPagedList(quoteFilterParams.PageNumber, quoteFilterParams.PageSize);

        var metadata = new
        {
            quotesPaginated.PageNumber,
            quotesPaginated.PageSize,
            quotesPaginated.PageCount,
            quotesPaginated.TotalItemCount,
            quotesPaginated.HasNextPage,
            quotesPaginated.HasPreviousPage,
        };

        var quotesDto = _mapper.Map<IEnumerable<QuoteResponseDTO>>(quotesEntities);

        return (quotesDto, metadata);
    }

    public async Task<QuoteDetailedResponseDTO> GetQuoteDetailsById(int id)
    {
        var quote = await GetDetailedQuoteDtoAsync(id);

        if (quote is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_NOT_FOUND);

        return quote;
    }

    private async Task<QuoteDetailedResponseDTO?> GetDetailedQuoteDtoAsync(int id)
    {
        var quote = await _unitOfWork.QuotesRepository.GetDetailedQuoteAsync(id);

        if (quote is not null)
        {
            var quoteDetailedDto = _mapper.Map<QuoteDetailedResponseDTO>(quote);

            return quoteDetailedDto;
        }

        return null;
    }

    private async Task<string> UploadInvoiceDocument(QuoteDetailedResponseDTO quote)
    {
        var invoiceDocument = _invoiceService.GenerateInvoiceDocument(quote);

        var fileName = $"invoice_{quote.CreatedAt:yyyyMMddTHHmmss}_{quote.QuoteId:d8}.pdf";

        var fileUrl = await _bucketService.UploadFileToS3(invoiceDocument, fileName);

        return fileUrl;
    }

    private async Task<string> GenerateInvoiceUrl(int id)
    {
        var quote = await GetDetailedQuoteDtoAsync(id);

        if (quote is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_NOT_FOUND);

        var generateInvoiceUrl = await UploadInvoiceDocument(quote);

        return generateInvoiceUrl;
    }

    private async Task SaveInvoiceOnQuote(int id)
    {
        var updatedQuote = await _unitOfWork.QuotesRepository.GetAsync(q => q.QuoteId == id);

        if (updatedQuote is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_NOT_FOUND);

        updatedQuote.FileUrl = await GenerateInvoiceUrl(id);

        _unitOfWork.QuotesRepository.Update(updatedQuote);

        await _unitOfWork.CommitAsync();
    }
}

