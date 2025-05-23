using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;

namespace CvnetClient.Utils {
	public class VdateConverter : IValueConverter {
		private static DateTime start_date = new(1901, 1, 1, 0, 0, 0); // 1901/01/01 00:00:00
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
			var decimalValue = (decimal)value;
			var date = start_date.AddDays((double)decimalValue);
			return date.ToString("yyyy/MM/dd HH:mm:ss"); // .fff
		}
		// 文字列をDateTimeに変換
		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) {
			string strValue = (string)value;
			DateTime resultDateTime;
			if (DateTime.TryParseExact(strValue, "yyyy/MM/dd HH:mm:ss", CultureInfo.CurrentCulture, DateTimeStyles.None, out resultDateTime)) {
				TimeSpan span = resultDateTime - start_date;
				var decimalValue = (decimal)span.TotalDays; // 現在時刻
				return decimalValue;
			}
			return DependencyProperty.UnsetValue;
		}
	}
}
