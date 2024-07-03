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

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class CustomerController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private ActionResult<IEnumerable<CustomerResponseDTO>> GetCustomers(IPagedList<Customer> customers)
    {
        var metadata = new
        {
            customers.Count,
            customers.PageSize,
            customers.PageCount,
            customers.TotalItemCount,
            customers.HasNextPage,
            customers.HasPreviousPage,
        };

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        var customerDto = _mapper.Map<IEnumerable<CustomerResponseDTO>>(customers);

        return Ok(customerDto);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IEnumerable<CustomerResponseDTO>>> GetAllCustomers([FromQuery] QueryParameters customerParams)
    {
        var customers = await _unitOfWork.CustomerRepository.GetCustomersAsync(customerParams);

        if (customers is null || customers.IsNullOrEmpty())
            return NotFound("Clientes não encontrados.");

        return GetCustomers(customers);
    }

    [HttpGet("{id:guid}", Name = "GetCustomerById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponseDTO>> GetCustomerById(Guid id)
    {
        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == id);

        if (customer is null)
            return NotFound("Cliente não encontrado.");

        var customerDto = _mapper.Map<CustomerResponseDTO>(customer);

        return Ok(customerDto);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponseDTO>> CreateCustomer(CustomerRequestDTO customerDto)
    {
        if (customerDto == null)
            return BadRequest("Digite os dados do cliente corretamente.");

        var customer = _mapper.Map<Customer>(customerDto);

        var createdCustomer = _unitOfWork.CustomerRepository.Create(customer);

        await _unitOfWork.CommitAsync();

        var newCustomer = _mapper.Map<CustomerResponseDTO>(createdCustomer);

        return new CreatedAtRouteResult("GetCustomerById", new { id = newCustomer.CustomerId }, newCustomer);
    }
}
