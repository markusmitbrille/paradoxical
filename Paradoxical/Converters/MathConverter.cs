using StringMath;
using System;
using System.Globalization;
using System.Windows.Data;

namespace Paradoxical.Converters
{
    /// <summary>
    /// </summary>
    public class MathConverter : IValueConverter
    {
        private const string FORMULA_SPLITTER = "|";
        private const string RESULT_JOINER = ",";
        private const string INPUT_VARIABLE_NAME = "x";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (parameter is not string formula)
            { throw new ArgumentException($"{nameof(parameter)} must be a string!", nameof(parameter)); }

            if (value is not double input)
            { throw new ArgumentException($"{nameof(value)} must be a double!", nameof(value)); }


            if (formula.Contains(FORMULA_SPLITTER))
            {
                string[] subformulas = formula.Split(FORMULA_SPLITTER);
                double[] results = new double[subformulas.Length];

                for (int i = 0; i < subformulas.Length; i++)
                {
                    string subformula = subformulas[i];
                    results[i] = Evaluate(subformula, input);
                }

                return string.Join(RESULT_JOINER, results);
            }
            else
            {
                return Evaluate(formula, input);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }

        private static double Evaluate(string formula, double input)
        {
            if (string.IsNullOrEmpty(formula))
            { return double.NaN; }

            if (formula.Contains(INPUT_VARIABLE_NAME))
            {
                return formula
                    .Replace(INPUT_VARIABLE_NAME, $"{{{INPUT_VARIABLE_NAME}}}")
                    .Substitute(INPUT_VARIABLE_NAME, input).Result;
            }
            else
            {
                return formula.Eval();
            }
        }
    }
}
