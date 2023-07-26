using System.Globalization;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules;

public class IntRangeRule : ValidationRule
{
    public int Min { get; set; } = 0;
    public int Max { get; set; } = 0;

    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        int number = 0;
        if (value is string text && int.TryParse(text, out number) == false)
        {
            return ValidationResult.ValidResult;
        }

        if (number < Min || number > Max)
        {
            return new ValidationResult(false, $"Needs to be between {Min} and {Max}.");
        }

        return ValidationResult.ValidResult;
    }
}
