using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using ServiceQuotes.DTOs.RequestDTO;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Models;
using ServiceQuotes.Repositories.Interfaces;

namespace ServiceQuotes.Controllers;
[Route("api/[controller]")]
[ApiController]
public class QuoteController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public QuoteController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    [HttpPost("CreateQuote")]
    public async Task<ActionResult<QuoteResponseDTO>> CreateQuote(QuoteRequestDTO quoteDto)
    {
        if (quoteDto is null)
            return BadRequest();

        var quote = _mapper.Map<Quote>(quoteDto);

        var createdQuote = _unitOfWork.QuotesRepository.Create(quote);

        await _unitOfWork.CommitAsync();

        var newQuoteDto = _mapper.Map<QuoteResponseDTO>(createdQuote);

        return Ok(newQuoteDto);
    }

    [HttpPost("AddProductsToQuote")]
    public async Task<ActionResult<QuoteResponseDTO>> AddProductsToQuote
        ([FromBody] AddProductsToQuoteRequestDTO quoteProductsDto)
    {
        if (quoteProductsDto.Products is null || quoteProductsDto.QuoteId <= 0)
        {
            return BadRequest();
        }

        var quote = await _unitOfWork.QuotesRepository.GetAsync(e => e.QuoteId == quoteProductsDto.QuoteId);

        if (quote is null)
            return NotFound();

        foreach (var product in quoteProductsDto.Products)
        {
            var productExists = await _unitOfWork.ProductRepository.GetAsync(e => e.ProductId == product.ProductId);

            if (productExists is null)
            {
                return NotFound("Product not found");
            }

            var productExistsOnQuote = await _unitOfWork.QuotesRepository
                .IsProductAssociatedWithQuote(productExists.ProductId, quote.QuoteId);

            if (productExistsOnQuote)
            {
                return Conflict("Product already exists on quote");
            }

            var quoteProduct = _mapper.Map<QuoteProducts>(product);
            quoteProduct.QuoteId = quoteProductsDto.QuoteId;

            quote.QuotesProducts!.Add(quoteProduct);
        }

        quote.TotalPrice = quote.QuotesProducts!.Sum(p => p.Price * p.Quantity);

        await _unitOfWork.CommitAsync();

        var quoteWithProductsDto = _mapper.Map<QuoteResponseDTO>(quote);

        return new CreatedAtRouteResult("GetQuoteDetails", new { id = quoteWithProductsDto.QuoteId }, quoteWithProductsDto);
    }

    [HttpGet("{id:int}", Name = "GetQuoteDetails")]
    public async Task<ActionResult<QuoteDetailedResponseDTO>> GetDetails(int id)
    {
        var getQuote = await _unitOfWork.QuotesRepository.GetDetailedQuoteAsync(id);

        if (getQuote is null)
            return NotFound();

        var quoteDetailedDto = _mapper.Map<QuoteDetailedResponseDTO>(getQuote);

        return Ok(quoteDetailedDto);
    }
}
