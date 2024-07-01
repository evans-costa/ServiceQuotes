using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using ServiceQuotes.DTOs.RequestDTO;
using ServiceQuotes.Models;
using ServiceQuotes.Pagination;
using ServiceQuotes.Repositories.Interfaces;
using X.PagedList;

namespace ServiceQuotes.Controllers;

[Route("api/[controller]")]
[ApiController]
public class CustomerController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CustomerController(IUnitOfWork unitOfWork, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    private ActionResult<IEnumerable<CustomerRequestDTO>> GetCustomers(IPagedList<Customer> customers)
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

        var customerDto = _mapper.Map<IEnumerable<CustomerRequestDTO>>(customers);

        return Ok(customerDto);
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CustomerRequestDTO>>> GetAll([FromQuery] QueryParameters customerParams)
    {
        var customers = await _unitOfWork.CustomerRepository.GetCustomersAsync(customerParams);

        if (customers is null || customers.IsNullOrEmpty())
        {
            return NotFound();
        }

        return GetCustomers(customers);
    }

    [HttpGet("{id:guid}", Name = "GetCustomer")]
    public async Task<ActionResult<CustomerRequestDTO>> GetById(Guid id)
    {
        var customer = await _unitOfWork.CustomerRepository.GetAsync(c => c.CustomerId == id);

        if (customer is null)
            return NotFound();

        var customerDto = _mapper.Map<CustomerRequestDTO>(customer);

        return Ok(customerDto);
    }

    [HttpPost]
    public async Task<ActionResult<CustomerRequestDTO>> Create(CustomerRequestDTO customerDto)
    {
        if (customerDto == null)
            return BadRequest();

        var customer = _mapper.Map<Customer>(customerDto);

        var createdCustomer = _unitOfWork.CustomerRepository.Create(customer);

        await _unitOfWork.CommitAsync();

        var newCustomer = _mapper.Map<CustomerRequestDTO>(createdCustomer);

        return new CreatedAtRouteResult("GetCustomer", new { id = newCustomer.CustomerId }, newCustomer);
    }
}
