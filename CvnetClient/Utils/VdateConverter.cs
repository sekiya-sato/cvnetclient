using System.Globalization; 
using System.Windows;
using System.Windows.Data;

namespace CvnetClient.Utils {
	public class VdateConverter : IValueConverter {
		private static DateTime start_date = new(1901, 1, 1, 0, 0, 0); // 1901/01/01 00:00:00
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) {
            if (value == null) return "1901/01/01 00:00:00";
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

    public static class VDateHelper
    {
        private static readonly DateTime StartDate = new(1901, 1, 1, 0, 0, 0);

        public static decimal ToVDate(DateTime date)
            => (decimal)(date - StartDate).TotalDays;

        public static DateTime FromVDate(decimal vdate)
            => StartDate.AddDays((double)vdate);

        public static double DateToValue(DateTime date_value)
        {
            TimeSpan span = date_value - StartDate;
            decimal span_d1 = (decimal)span.TotalDays; // 現在時刻
            double span_d2 = (double)span_d1;
            return span_d2;
        }
        /// <summary>
        /// 日付を表すvdate値から日付を求める
        /// </summary>
        /// <param name="date_value"></param>
        /// <returns></returns>
        public static DateTime DateFromValue(double date_value)
        {
            return StartDate.AddDays(date_value);
        }
        /// <summary>
        /// <para>null値をToStringする</para>
        /// <para>nullの場合、空を返す</para>
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <returns>書式設定された値または空</returns>
        /// 


        public static double DateUpdate(double date_value)
        {
            #region Old Source...
            // Win2K以上では1ミリ秒単位で値が返ってくるので、ほぼ不要
            /*
            TimeSpan span= DateTime.Now - start_date;
            double span_d1=span.TotalDays; // 現在時刻
            if (date_value < span_d1) return span_d1;
            double span_sec = 0.00000001; // この刻み幅で 1000回/秒までの更新回数
            TimeSpan span2= new TimeSpan(span.Days,span.Hours,span.Minutes,
                span.Seconds+1,span.Milliseconds);
            double span_d2=span2.TotalDays-span_sec; // 現在時刻+1秒-span_sec
            if( date_value >= span_d1 && date_value < span_d2) return (date_value+span_sec);
             */
            #endregion
            TimeSpan span = DateTime.Now - StartDate;
            decimal span_d1 = (decimal)span.TotalDays; // 現在時刻
            double span_d2 = (double)span_d1;
            if (span_d2 == date_value) span_d2 = span_d2 + 0.00000001d;
            return span_d2;
        }

        public static DateTime ConvToDate(string in_str)
        {
            DateTime ret_val = StartDate;
            try
            {
                int d_year = Convert.ToInt32(in_str.Substring(0, 4));
                int d_mon = Convert.ToInt32(in_str.Substring(4, 2));
                int d_day = Convert.ToInt32(in_str.Substring(6, 2));
                ret_val = new DateTime(d_year, d_mon, d_day);
            }
            catch (Exception)
            {
                return StartDate;
            }
            return ret_val;
        }

        public static double ConvTimeString(string time_str)
        {
            long[] time_value = { 0, 0, 0 };
            if (time_str.Length < 6) return (double)0;
            for (int i = 0; i < 3; i++)
            {
                string wrk_str = time_str.Substring(i * 2, 2);
                if (wrk_str.Trim() != "")
                {
                    try
                    {
                        time_value[i] = Convert.ToInt64(wrk_str, 10);
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            double ret_val;
            ret_val = time_value[0] * 60 * 60 + time_value[1] * 60 + time_value[2];
            ret_val /= (24 * 60 * 60);
            return ret_val;
        }


        public static string ToStringOrEmpty(this object? value, string format = "")
        {
            if (value != null)
            {
                return string.Format("{0:" + format + "}", value);
            }
            return string.Empty;
        }

        public static DateTime ConvertJapaneseDate(string input)
        {
            string[] parts = input.Split('年', '月', '日');

            if (parts.Length == 4 &&
                int.TryParse(parts[0], out int year) &&
                int.TryParse(parts[1], out int month) &&
                int.TryParse(parts[2], out int day))
            {
                return new DateTime(year, month, day);
            }

            return DateTime.MinValue;
        }
    }
}
