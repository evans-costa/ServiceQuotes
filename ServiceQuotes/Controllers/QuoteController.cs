using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.Quote;
using ServiceQuotes.Exceptions;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using ServiceQuotes.Resources;
using ServiceQuotes.Services;
using System.Net.Mime;
using X.PagedList;

namespace ServiceQuotes.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[ApiController]
public class QuoteController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<QuoteController> _logger;
    private readonly IInvoiceService _invoiceService;
    private readonly IS3BucketService _bucketService;

    public QuoteController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<QuoteController> logger, IInvoiceService invoiceService, IS3BucketService bucketService)
    {
        _unitOfWork = unitOfWork;
        _invoiceService = invoiceService;
        _logger = logger;
        _mapper = mapper;
        _bucketService = bucketService;
    }

    private ActionResult<IEnumerable<QuoteResponseDTO>> GetQuotes(IPagedList<Quote> quotes)
    {
        var metadata = new
        {
            quotes.Count,
            quotes.PageSize,
            quotes.PageCount,
            quotes.TotalItemCount,
            quotes.HasNextPage,
            quotes.HasPreviousPage,
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var quoteDto = _mapper.Map<IEnumerable<QuoteResponseDTO>>(quotes);

        return Ok(quoteDto);
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuoteResponseDTO>>> GetAllQuotes([FromQuery] QueryParameters quoteParams)
    {
        _logger.LogInformation("### Get all quotes: GET api/qutoe/ ###");

        var quotes = await _unitOfWork.QuotesRepository.GetQuotesAsync(quoteParams);

        if (quotes is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_NOT_FOUND);

        return GetQuotes(quotes);
    }

    [HttpGet("{id:int}", Name = "GetQuoteDetailsById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuoteDetailedResponseDTO>> GetQuoteDetailsById(int id)
    {
        _logger.LogInformation("### Get a detailed quote by ID: GET api/quote/{id} ###", id);

        var quote = await GetDetailedQuoteDtoAsync(id);

        if (quote is null)
            throw new NotFoundException(ExceptionMessages.QUOTE_NOT_FOUND);

        return Ok(quote);
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuoteResponseDTO>>> GetQuoteBySearch([FromQuery] QuoteFilterParams quoteParams)
    {
        _logger.LogInformation("### Get a quote by search criteria: GET api/quote/search/{quoteParams} ###", quoteParams);

        var quotes = await _unitOfWork.QuotesRepository.SearchQuotesAsync(quoteParams);

        if (quotes is null || quotes.IsNullOrEmpty())
            throw new NotFoundException(ExceptionMessages.QUOTE_SEARCH_NOT_FOUND);

        return GetQuotes(quotes);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<QuoteResponseDTO>> CreateQuote
        ([FromBody] QuoteWithProductRequestDTO quoteWithProductsDto)
    {
        _logger.LogInformation("### Create a quote: POST api/quote");

        if (quoteWithProductsDto is null || quoteWithProductsDto is { Products: null } or { Quote: null })
            return BadRequest();

        var quote = _mapper.Map<Quote>(quoteWithProductsDto.Quote);

        foreach (var product in quoteWithProductsDto.Products)
        {
            var productExists = await _unitOfWork.ProductRepository.GetAsync(e => e.ProductId == product.ProductId);

            if (productExists is null)
                throw new NotFoundException(ExceptionMessages.PRODUCT_NOT_FOUND);

            if (quote.QuotesProducts.Any(qp => qp.ProductId == product.ProductId))
            {
                _logger.LogWarning("Product {product} already in this quote", productExists.Name);
                return Conflict("Produto duplicado no orçamento.");
            }

            var quoteProduct = _mapper.Map<QuoteProducts>(product);

            quote.QuotesProducts.Add(quoteProduct);
        }

        quote.TotalPrice = quote.QuotesProducts.Sum(p => p.Price * p.Quantity);

        _unitOfWork.QuotesRepository.Create(quote);

        await _unitOfWork.CommitAsync();

        await SaveInvoiceOnQuote(quote.QuoteId);

        var newQuoteDto = _mapper.Map<QuoteResponseDTO>(quote);

        return new CreatedAtRouteResult("GetQuoteDetailsById", new { id = newQuoteDto.QuoteId }, newQuoteDto);
    }
}
