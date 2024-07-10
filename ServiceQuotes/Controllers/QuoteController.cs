using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.Quote;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
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

    public QuoteController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<QuoteController> logger, IInvoiceService invoiceService)
    {
        _unitOfWork = unitOfWork;
        _invoiceService = invoiceService;
        _logger = logger;
        _mapper = mapper;
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

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuoteResponseDTO>>> GetAllQuotes([FromQuery] QueryParameters quoteParams)
    {
        _logger.LogInformation("### Get all quotes: GET api/qutoe/ ###");

        var quotes = await _unitOfWork.QuotesRepository.GetQuotesAsync(quoteParams);

        if (quotes is null)
        {
            _logger.LogWarning("Quotes not found.");
            return NotFound("Orçamentos não encontrados");
        }

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
        {
            _logger.LogWarning("Quote with {id} not found.", id);
            return NotFound("Orçamento não encontrado");
        }

        return Ok(quote);
    }

    [HttpGet("{id:int}/invoice")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoiceByQuoteId(int id)
    {
        _logger.LogInformation("### Get an invoice document by quote ID: GET api/quote/{id}/invoice ###", id);

        var quote = await GetDetailedQuoteDtoAsync(id);

        if (quote is null)
        {
            _logger.LogWarning("Quote with {id} not found.", id);
            return NotFound("Orçamento não encontrado");
        }

        var invoiceDocument = _invoiceService.GenerateInvoiceDocument(quote);

        return File(invoiceDocument, "application/pdf",
            $"invoice_{quote.CreatedAt:yyyyMMddTHHmmss}_{id:d8}.pdf");
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuoteResponseDTO>>> GetQuoteBySearch([FromQuery] QuoteFilterParams quoteParams)
    {
        _logger.LogInformation("### Get a quote by search criteria: GET api/quote/search/{quoteParams} ###", quoteParams);

        var quotes = await _unitOfWork.QuotesRepository.SearchQuotesAsync(quoteParams);

        if (quotes is null || quotes.IsNullOrEmpty())
        {
            _logger.LogWarning("Quote not found by informed {quoteParams} search criteria", quoteParams);
            return NotFound("Orçamento não encontrado pelos critérios desejados.");
        }

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
            {
                _logger.LogWarning("Product not found.");
                return NotFound("Produto não encontrado.");
            }

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

        var newQuoteDto = _mapper.Map<QuoteResponseDTO>(quote);

        return new CreatedAtRouteResult("GetQuoteDetailsById", new { id = newQuoteDto.QuoteId }, newQuoteDto);
    }
}
