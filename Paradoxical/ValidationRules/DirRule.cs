using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows.Controls;

namespace Paradoxical.ValidationRules;

public class DirRule : ValidationRule
{
    public override ValidationResult Validate(object value, CultureInfo cultureInfo)
    {
        char[] invalids = Path.GetInvalidPathChars();

        if (value is string file && invalids.Any(file.Contains) == true)
        {
            return new ValidationResult(false, $"Contains an invalid character: '{invalids.First(file.Contains)}'");
        }

        return ValidationResult.ValidResult;
    }
}
