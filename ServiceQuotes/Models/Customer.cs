using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;

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
    [StringLength(80)]
    public string? Name { get; set; }
    public string? Phone { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public ICollection<Quote>? Quotes { get; }
}
