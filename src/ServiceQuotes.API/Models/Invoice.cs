namespace ServiceQuotes.API.Models;

public class Invoice
{
    public int InvoiceNumber { get; set; }
    public DateTime CreatedAt { get; set; }

    public CustomerInfo? CustomerInfo { get; set; }

    public List<OrderItem>? Items { get; set; }
}

public class OrderItem
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public int? Quantity { get; set; }
}

public class CustomerInfo
{
    public string? Name { get; set; }
    public string? Phone { get; set; }
}

