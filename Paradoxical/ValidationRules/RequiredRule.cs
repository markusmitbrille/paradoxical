using System.Globalization;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules;

public class RequiredRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null)
        {
            return new ValidationResult(false, "Field is required.");
        }

        if (value is string text && text == string.Empty)
        {
            return new ValidationResult(false, "Field is required.");
        }

        return ValidationResult.ValidResult;
    }
}
