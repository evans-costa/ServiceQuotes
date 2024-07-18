using AutoMapper;
using ServiceQuotes.Application.DTO.Product;
using ServiceQuotes.Application.Exceptions;
using ServiceQuotes.Application.Exceptions.Resources;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Domain.Pagination;
using X.PagedList.Extensions;

namespace ServiceQuotes.Application.Services;
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public ProductService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productDto)
    {
        var product = _mapper.Map<Product>(productDto);

        var createdProduct = await _unitOfWork.ProductRepository.CreateAsync(product);

        await _unitOfWork.CommitAsync();

        return _mapper.Map<ProductResponseDTO>(createdProduct);
    }

    public async Task<(IEnumerable<ProductResponseDTO>, object)> GetAllProducts(QueryParameters productParams)
    {
        var productEntities = await _unitOfWork.ProductRepository.GetProductsAsync();

        if (productEntities is null)
            throw new NotFoundException(ExceptionMessages.PRODUCT_NOT_FOUND);

        var productsPaginated = productEntities.ToPagedList(productParams.PageNumber, productParams.PageSize);

        var metadata = new
        {
            productsPaginated.PageNumber,
            productsPaginated.PageSize,
            productsPaginated.PageCount,
            productsPaginated.TotalItemCount,
            productsPaginated.HasNextPage,
            productsPaginated.HasPreviousPage,
        };

        var productDto = _mapper.Map<IEnumerable<ProductResponseDTO>>(productsPaginated);

        return (productDto, metadata);
    }

    public async Task<ProductResponseDTO> GetProductById(Guid id)
    {
        var productEntity = await _unitOfWork.ProductRepository.GetAsync(p => p.ProductId == id);

        if (productEntity is null)
            throw new NotFoundException(ExceptionMessages.PRODUCT_NOT_FOUND);

        return _mapper.Map<ProductResponseDTO>(productEntity);
    }
}
