using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ServiceQuotes.Models;

public class Customer
{
    public Customer()
    {
        Quotes = new Collection<Quote>();
    }

    [Key]
    public Guid CustomerId { get; set; }

    [Required]
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    [JsonIgnore]
    public ICollection<Quote> Quotes { get; }
}
