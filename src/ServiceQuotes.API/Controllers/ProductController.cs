using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceQuotes.Application.DTO.Product;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Domain.Pagination;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace ServiceQuotes.API.Controllers;

[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductController> _logger;

    public ProductController(IProductService productService, ILogger<ProductController> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Criar um produto")]
    public async Task<ActionResult<ProductResponseDTO>> CreateProduct(ProductRequestDTO productDto)
    {
        _logger.LogInformation("### Create a product: POST api/product ###");

        var newProduct = await _productService.CreateProduct(productDto);

        return new CreatedAtRouteResult("GetProductById", new
        {
            id = newProduct.ProductId,
        }, newProduct);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Buscar todas os produtos")]
    public async Task<ActionResult<IEnumerable<ProductResponseDTO>>> GetAllProducts([FromQuery] QueryParameters productParams)
    {
        _logger.LogInformation("### Get all products: GET api/product ###");

        var (products, metadata) = await _productService.GetAllProducts(productParams);

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        return Ok(products);
    }

    [HttpGet("{id:guid}", Name = "GetProductById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Buscar um produto por ID")]
    public async Task<ActionResult<ProductResponseDTO>> GetProductById(Guid id)
    {
        _logger.LogInformation("### Get a product by ID: GET api/product/{id} ###", id);

        var product = await _productService.GetProductById(id);

        return Ok(product);
    }
}
