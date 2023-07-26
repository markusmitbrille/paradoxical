using System.Globalization;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules;

public class IntRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value is string text && int.TryParse(text, out _) == false)
        {
            return new ValidationResult(false, "Needs to be an integer.");
        }

        return ValidationResult.ValidResult;
    }
}
