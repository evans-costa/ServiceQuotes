using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.RequestDTO;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using ServiceQuotes.Services;
using System.Net.Mime;
using X.PagedList;

namespace ServiceQuotes.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class QuoteController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IInvoiceService _invoiceService;

    public QuoteController(IUnitOfWork unitOfWork, IMapper mapper, IInvoiceService invoiceService)
    {
        _unitOfWork = unitOfWork;
        _invoiceService = invoiceService;
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

    [HttpGet("{id:int}", Name = "GetQuoteDetailsById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuoteDetailedResponseDTO>> GetQuoteDetailsById(int id)
    {
        var quote = await GetDetailedQuoteDtoAsync(id);

        if (quote is null)
            return NotFound("Orçamento não encontrado");

        return Ok(quote);
    }

    [HttpGet("{id:int}/invoice")]
    [Produces("application/pdf")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetInvoiceByQuoteId(int id)
    {
        var quote = await GetDetailedQuoteDtoAsync(id);

        if (quote is null)
            return NotFound("Orçamento não encontrado");

        var invoiceDocument = _invoiceService.GenerateInvoiceDocument(quote);

        return File(invoiceDocument, "application/pdf",
            $"invoice_{quote.CreatedAt:yyyyMMddTHHmmss}_{id:d8}.pdf");
    }

    [HttpGet("search")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<QuoteResponseDTO>>> GetQuoteBySearch([FromQuery] QuoteFilterParams quoteParams)
    {
        var quotes = await _unitOfWork.QuotesRepository.SearchQuotesAsync(quoteParams);

        if (quotes is null || quotes.IsNullOrEmpty())
        {
            return NotFound("Orçamento não encontrado pelos critérios desejados.");
        }

        return GetQuotes(quotes);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<QuoteResponseDTO>> AddProductsToQuote
        ([FromBody] QuoteWithProductRequestDTO quoteWithProductsDto)
    {
        if (quoteWithProductsDto.Products is null || quoteWithProductsDto.Quote is null || quoteWithProductsDto is null)
            return BadRequest("Informe os dados do orçamento corretamente.");

        var quote = _mapper.Map<Quote>(quoteWithProductsDto.Quote);

        foreach (var product in quoteWithProductsDto.Products)
        {
            var productExists = await _unitOfWork.ProductRepository.GetAsync(e => e.ProductId == product.ProductId);

            if (productExists is null)
                return NotFound("Produto não encontrado");

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
