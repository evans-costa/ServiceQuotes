using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.ResponseDTO;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using X.PagedList;

namespace ServiceQuotes.Controllers;

[Route("api/[controller]")]
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

    private ActionResult<IEnumerable<ProductDTO>> GetProducts(IPagedList<Product> product)
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

        var productDto = _mapper.Map<IEnumerable<ProductDTO>>(product);

        return Ok(productDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDTO>>> GetAll([FromQuery] QueryParameters productParams)
    {
        var products = await _unitOfWork.ProductRepository.GetProductsAsync(productParams);

        if (products is null || products.IsNullOrEmpty())
        {
            return NotFound();
        }

        return GetProducts(products);
    }

    [HttpGet("{id:guid}", Name = "GetProduct")]
    public async Task<ActionResult<ProductDTO>> GetById(Guid
        id)
    {
        var product = await _unitOfWork.ProductRepository.GetAsync(p => p.ProductId == id);

        if (product is null)
            return NotFound();

        var productDto = _mapper.Map<ProductDTO>(product);

        return Ok(productDto);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDTO>> Create(ProductDTO productDto)
    {
        if (productDto is null)
        {
            return BadRequest();
        }

        var product = _mapper.Map<Product>(productDto);

        var createdProduct = _unitOfWork.ProductRepository.Create(product);

        await _unitOfWork.CommitAsync();

        var newProduct = _mapper.Map<ProductDTO>(createdProduct);

        return new CreatedAtRouteResult("GetProduct", new
        {
            id = newProduct.ProductId
        }, newProduct);
    }
}
