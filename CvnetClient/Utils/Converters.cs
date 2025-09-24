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
}
