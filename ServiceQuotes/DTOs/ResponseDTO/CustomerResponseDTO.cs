namespace ServiceQuotes.DTOs.ResponseDTO;

public class CustomerResponseDTO
{
    public Guid CustomerId { get; set; }
    public string? Name { get; set; }
    public string? Phone { get; set; }
}
