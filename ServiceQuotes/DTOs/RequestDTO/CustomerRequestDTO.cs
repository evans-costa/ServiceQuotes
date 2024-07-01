namespace ServiceQuotes.DTOs.RequestDTO;

public class CustomerRequestDTO
{
    public Guid CustomerId { get; set; }
    public string Name { get; set; }
    public string? Phone { get; set; }
}
