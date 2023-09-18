using Paradoxical.Extensions;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules;

public partial class ScopeRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        if (value == null)
        {
            return ValidationResult.ValidResult;
        }

        if (value is not string text)
        {
            return new ValidationResult(false, "Needs to be a string.");
        }

        if (text.IsEmpty() == true)
        {
            return ValidationResult.ValidResult;
        }

        if (GetScopeRegex().IsMatch(text) == false)
        {
            return new ValidationResult(false, "Needs to be a valid scope.");
        }

        return ValidationResult.ValidResult;
    }

    [GeneratedRegex(@"^[a-zA-Z][a-zA-Z0-9_]*$")]
    private static partial Regex GetScopeRegex();
}
