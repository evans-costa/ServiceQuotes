using AutoMapper;
using ServiceQuotes.Application.DTO.Customer;
using ServiceQuotes.Application.Exceptions;
using ServiceQuotes.Application.Exceptions.Resources;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Domain.Entities;
using ServiceQuotes.Domain.Interfaces;
using ServiceQuotes.Domain.Pagination;
using X.PagedList.Extensions;

namespace ServiceQuotes.Application.Services;
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerService(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<CustomerResponseDTO> CreateCustomer(CustomerRequestDTO customerDto)
    {
        var customer = _mapper.Map<Customer>(customerDto);

        var createdCustomer = await _unitOfWork.CustomerRepository.CreateAsync(customer);

        await _unitOfWork.CommitAsync();

        return _mapper.Map<CustomerResponseDTO>(createdCustomer);
    }

    public async Task<(IEnumerable<CustomerResponseDTO>, object)> GetAllCustomers(QueryParameters customerParams)
    {
        var customerEntities = await _unitOfWork.CustomerRepository.GetCustomersAsync();

        if (customerEntities is null)
            throw new NotFoundException(ExceptionMessages.CUSTOMER_NOT_FOUND);

        var customerPaginated = customerEntities.ToPagedList(customerParams.PageNumber, customerParams.PageSize);

        var customerDto = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customerPaginated);

        var metadata = new
        {
            customerPaginated.PageNumber,
            customerPaginated.PageSize,
            customerPaginated.PageCount,
            customerPaginated.TotalItemCount,
            customerPaginated.HasNextPage,
            customerPaginated.HasPreviousPage,
        };

        return (customerDto, metadata);
    }

    public async Task<CustomerResponseDTO> GetCustomerById(Guid id)
    {
        var customerEntity = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == id);

        if (customerEntity is null)
            throw new NotFoundException(ExceptionMessages.CUSTOMER_NOT_FOUND);

        return _mapper.Map<CustomerResponseDTO>(customerEntity);
    }
}
