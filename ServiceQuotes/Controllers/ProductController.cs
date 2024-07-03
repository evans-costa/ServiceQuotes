using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.RequestDTO;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using System.Net.Mime;
using X.PagedList;

namespace ServiceQuotes.Controllers;

[Route("api/[controller]")]
[Produces("application/json")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    private ActionResult<IEnumerable<ProductResponseDTO>> GetProducts(IPagedList<Product> product)
    {
        var metadata = new
        {
            product.Count,
            product.PageNumber,
            product.PageSize,
            product.TotalItemCount,
            product.HasNextPage,
            product.HasPreviousPage,
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var productDto = _mapper.Map<IEnumerable<ProductResponseDTO>>(product);

        return Ok(productDto);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAllProducts([FromQuery] QueryParameters productParams)
    {
        var products = await _unitOfWork.ProductRepository.GetProductsAsync(productParams);

        if (products is null || products.IsNullOrEmpty())
            return NotFound("Produtos não encontrados.");

        return GetProducts(products);
    }

    [HttpGet("{id:guid}", Name = "GetProductById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductResponseDTO>> GetProductById(Guid id)
    {
        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.ProductId == id);

        if (product is null)
            return NotFound("Produto não encontrado.");

        var productDto = _mapper.Map<ProductResponseDTO>(product);

        return Ok(productDto);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductResponseDTO>> CreateProduct(ProductRequestDTO productDto)
    {
        if (productDto is null)
        {
            return BadRequest("Digite os dados do produto corretamente.");
        }

        var product = _mapper.Map<Product>(productDto);

        var createdProduct = _unitOfWork.ProductRepository.Create(product);

        await _unitOfWork.CommitAsync();

        var newProduct = _mapper.Map<ProductResponseDTO>(createdProduct);

        return new CreatedAtRouteResult("GetProductById", new
        {
            id = newProduct.ProductId
        }, newProduct);
    }
}
