using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.Domain.Entities;

public sealed class Customer
{
    public Customer()
    {
        Quotes = new Collection<Quote>();
    }

    [Key]
    public Guid CustomerId { get; set; }

    [Required]
    public string? Name { get; private set; }
    [StringLength(20)]
    public string? Phone { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public DateTime UpdatedAt { get; private set; }
    public ICollection<Quote> Quotes { get; }
}
