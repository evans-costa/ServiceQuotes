using ServiceQuotes.Application.DTO.Product;
using ServiceQuotes.Domain.Pagination;

namespace ServiceQuotes.Application.Interfaces;
public interface IProductService
{
    Task<ProductResponseDTO> CreateProduct(ProductRequestDTO productDto);
    Task<(IEnumerable<ProductResponseDTO>, object)> GetAllProducts(QueryParameters productParams);
    Task<ProductResponseDTO> GetProductById(Guid id);
}
