using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.Customer;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using System.Net.Mime;
using X.PagedList;

namespace ServiceQuotes.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CustomerController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CustomerController> logger)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _logger = logger;
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
        _logger.LogInformation("### Get all customers: GET api/customer ###");

        var customers = await _unitOfWork.CustomerRepository.GetCustomersAsync(customerParams);

        if (customers is null || customers.IsNullOrEmpty())
        {
            _logger.LogWarning("Customers not found.");
            return NotFound("Clientes não encontrados.");
        }

        return GetCustomers(customers);
    }

    [HttpGet("{id:guid}", Name = "GetCustomerById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CustomerResponseDTO>> GetCustomerById(Guid id)
    {
        _logger.LogInformation("### Get a customer by ID: GET api/customer/{id} ###", id);

        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == id);

        if (customer is null)
        {
            _logger.LogWarning("Customer with {id} not found.", id);
            return NotFound("Cliente não encontrado.");
        }


        var customerDto = _mapper.Map<CustomerResponseDTO>(customer);

        return Ok(customerDto);
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CustomerResponseDTO>> CreateCustomer(CustomerRequestDTO customerDto)
    {
        _logger.LogInformation("### Create a customer: POST api/customer ###");

        if (customerDto == null)
        {
            return BadRequest();
        }

        var customer = _mapper.Map<Customer>(customerDto);

        var createdCustomer = _unitOfWork.CustomerRepository.Create(customer);

        await _unitOfWork.CommitAsync();

        var newCustomer = _mapper.Map<CustomerResponseDTO>(createdCustomer);

        return new CreatedAtRouteResult("GetCustomerById", new { id = newCustomer.CustomerId }, newCustomer);
    }
}
