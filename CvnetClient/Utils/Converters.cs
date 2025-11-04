using System.Globalization; 
using System.Windows.Data;

namespace CvnetClient.Utils
{
    public class RdIntToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null)
                return false;
            return value.ToString() == parameter.ToString(); 
            //int.TryParse(parameter?.ToString(), out var result) ? result : 0; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value && int.TryParse(parameter.ToString(), out int result))
                return result;

            return Binding.DoNothing;
        }
    }

    public class SubtractHeaderFooterHeightConverter : IValueConverter
    {
        public double Offset { get; set; } = 0; // default = 0, but parameter overrides it

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            double offsetToUse = Offset;

            if (parameter != null && double.TryParse(parameter.ToString(), out double paramOffset))
            {
                offsetToUse = paramOffset;
            }

            if (value is double height)
            {
                var result = Math.Max(0, height - offsetToUse);
                System.Diagnostics.Debug.WriteLine(
                    $"[SubtractHeaderFooterHeightConverter] Window height={height}, Offset={offsetToUse}, Result={result}");
                return result;
            }

            return 0;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    public class TextBoxMultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            var code = values[0]?.ToString() ?? "";
            var name = values[1]?.ToString() ?? "";
            return string.IsNullOrWhiteSpace(name) ? code : $"{code} {name}";
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            var text = value?.ToString() ?? "";
            string code = text;
            string name = null;

            var parts = text.Split(new[] { ' ' }, 2, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length > 0)
                code = parts[0];
            if (parts.Length > 1)
                name = parts[1];

            return new object[] { code, name ?? Binding.DoNothing };
        }
    }
    public class PercentageBelowConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is double d)
            {
                return d.ToString("00.0") + "%以下";
            }
            return string.Empty;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var str = value?.ToString()?.Replace("%以下", "").Trim();
            if (double.TryParse(str, out double result))
            {
                return result;
            }
            return null;
        }
    }

    public class EnumToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;
            return value.ToString() == parameter.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((bool)value && parameter != null)
            {
                return Enum.Parse(targetType, parameter.ToString());
            }
            return Binding.DoNothing;
        }
    }

}
