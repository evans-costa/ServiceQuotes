using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ServiceQuotes.Application.DTO.Customer;
using ServiceQuotes.Application.Interfaces;
using ServiceQuotes.Domain.Pagination;
using Swashbuckle.AspNetCore.Annotations;
using System.Net.Mime;

namespace ServiceQuotes.API.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/[controller]")]
[Produces("application/json")]
public class CustomerController : ControllerBase
{
    private readonly ICustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpPost]
    [Consumes(MediaTypeNames.Application.Json)]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [SwaggerOperation(Summary = "Criar um cliente")]
    public async Task<ActionResult<CustomerResponseDTO>> CreateCustomer(CustomerRequestDTO customerDto)
    {
        _logger.LogInformation("### Create a customer: POST api/customer ###");

        var newCustomer = await _customerService.CreateCustomer(customerDto);

        return new CreatedAtRouteResult("GetCustomerById", new { id = newCustomer.CustomerId }, newCustomer);
    }

    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Buscar todos os clientes")]
    public async Task<ActionResult<IEnumerable<CustomerResponseDTO>>> GetAllCustomers([FromQuery] QueryParameters customerParams)
    {
        _logger.LogInformation("### Get all customers: GET api/customer ###");

        var (customers, metadata) = await _customerService.GetAllCustomers(customerParams);

        Response.Headers.Append("X-Pagination", JsonConvert.SerializeObject(metadata));

        return Ok(customers);
    }

    [HttpGet("{id:guid}", Name = "GetCustomerById")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [SwaggerOperation(Summary = "Buscar um cliente pelo ID")]
    public async Task<ActionResult<CustomerResponseDTO>> GetCustomerById(Guid id)
    {
        _logger.LogInformation("### Get a customer by ID: GET api/customer/{id} ###", id);

        var customer = await _customerService.GetCustomerById(id);

        return Ok(customer);
    }
}
