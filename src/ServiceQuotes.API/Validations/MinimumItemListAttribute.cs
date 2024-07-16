using System.Collections;
using System.ComponentModel.DataAnnotations;

namespace ServiceQuotes.API.Validations;

public class MinimumItemListAttribute : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var productList = value as ICollection;

        if (productList?.Count == 0 || productList is null)
        {
            return new ValidationResult("O orçamento deve ter pelo menos 1 produto");
        }

        return ValidationResult.Success;
    }
}
