using System.Windows;
using System.Windows.Controls;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for CustomDateView.xaml
    /// </summary>
    public partial class CustomDateView : UserControl
    {
        public CustomDateView()
        {
            InitializeComponent();
            GenerateTimeItems();
        }

        #region Dependency Property 
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(
                nameof(Text),
                typeof(string),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(
                nameof(Format),
                typeof(string),
                typeof(CustomDateView),
                new PropertyMetadata("yyyy/MM/dd"));
        public string Format
        {
            get => (string)GetValue(FormatProperty);
            set => SetValue(FormatProperty, value);
        }

        public static readonly DependencyProperty SelectedDateTimeProperty =
            DependencyProperty.Register(
                nameof(SelectedDateTime),
                typeof(DateTime?),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedDateTimeChanged));
        public DateTime? SelectedDateTime
        {
            get => (DateTime?)GetValue(SelectedDateTimeProperty);
            set => SetValue(SelectedDateTimeProperty, value);
        }

        public static readonly DependencyProperty IsPopupOpenProperty =
            DependencyProperty.Register(
                nameof(IsPopupOpen),
                typeof(bool),
                typeof(CustomDateView),
                new PropertyMetadata(false));
        public bool IsPopupOpen
        {
            get => (bool)GetValue(IsPopupOpenProperty);
            set => SetValue(IsPopupOpenProperty, value);
        }

        public static readonly DependencyProperty HourListProperty =
            DependencyProperty.Register(
                nameof(HourList),
                typeof(Dictionary<string, string>),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Dictionary<string, string> HourList
        {
            get => (Dictionary<string, string>)GetValue(HourListProperty);
            set => SetValue(HourListProperty, value);
        }

        public static readonly DependencyProperty IsHourProperty =
            DependencyProperty.Register(
                nameof(IsHour),
                typeof(string),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string IsHour
        {
            get => (string)GetValue(IsHourProperty);
            set => SetValue(IsHourProperty, value);
        }
         
        public static readonly DependencyProperty MinsListProperty =
            DependencyProperty.Register(
                nameof(MinsList),
                typeof(Dictionary<string, string>),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Dictionary<string, string> MinsList
        {
            get => (Dictionary<string, string>)GetValue(MinsListProperty);
            set => SetValue(MinsListProperty, value);
        }

        public static readonly DependencyProperty IsMinsProperty =
            DependencyProperty.Register(
                nameof(IsMins),
                typeof(string),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string IsMins
        {
            get => (string)GetValue(IsMinsProperty);
            set => SetValue(IsMinsProperty, value);
        }

        public static readonly DependencyProperty SecsListProperty =
            DependencyProperty.Register(
                nameof(SecsList),
                typeof(Dictionary<string, string>),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public Dictionary<string, string> SecsList
        {
            get => (Dictionary<string, string>)GetValue(SecsListProperty);
            set => SetValue(SecsListProperty, value);
        }

        public static readonly DependencyProperty IsSecsProperty =
            DependencyProperty.Register(
                nameof(IsSecs),
                typeof(string),
                typeof(CustomDateView),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string IsSecs
        {
            get => (string)GetValue(IsSecsProperty);
            set => SetValue(IsSecsProperty, value);
        }
        #endregion

        #region Events
        private static void OnSelectedDateTimeChanged(
                DependencyObject d,
                DependencyPropertyChangedEventArgs e)
        {
            var control = (CustomDateView)d;

            if (e.NewValue is DateTime dt)
            {
                control.ApplyDateTime(dt);
            }
            else
            {
                control.Text = string.Empty;
            }
        }

        private void Confirm(object sender, RoutedEventArgs e)
        {
            if (Calendar.SelectedDate == null ||
                string.IsNullOrEmpty(IsHour)  || 
                string.IsNullOrEmpty(IsMins)  ||
                string.IsNullOrEmpty(IsSecs)) return; 
            try
            {
                int hh = int.Parse(IsHour);
                int mm = int.Parse(IsMins);
                int ss = int.Parse(IsSecs);

                SelectedDateTime = Calendar.SelectedDate.Value.Date
                                            .AddHours(hh)
                                            .AddMinutes(mm)
                                            .AddSeconds(ss);

                var format = GetValidFormat();
                Text = SelectedDateTime.Value.ToString(format);

                IsPopupOpen = false;
            }
            catch { }
        }
        #endregion

        #region Function  
        private string GetValidFormat()
        {
            try
            {
                DateTime.Now.ToString(Format);
                return Format;
            }
            catch
            {
                return "yyyy/MM/dd";
            }
        }

        private void GenerateTimeItems()
        {
            if (HourList == null || HourList.Count == 0)
            {
                HourList = new Dictionary<string, string>();
                // HH (00–23)
                for (int h = 0; h < 24; h++)
                {
                    HourList.Add(h.ToString("00"), h.ToString("00"));
                }
            }

            if (MinsList == null || MinsList.Count == 0)
            {
                MinsList = new Dictionary<string, string>();
                // mm (00–59)
                for (int m = 0; m < 60; m++)
                    MinsList.Add(m.ToString("00"), m.ToString("00"));
            }

            if (SecsList == null || SecsList.Count == 0)
            {
                SecsList = new Dictionary<string, string>();
                // ss (00–59)
                for (int s = 0; s < 60; s++)
                    SecsList.Add(s.ToString("00"), s.ToString("00"));
            }

            HourComboBox.SelectedIndex = 0;
            MinuteComboBox.SelectedIndex = 0;
            SecondComboBox.SelectedIndex = 0;
        }

        private void OpenPopup(object sender, RoutedEventArgs e)
        {
            IsPopupOpen = true;
        }

        private void ApplyDateTime(DateTime dt)
        {
            // Update UI controls
            Calendar.SelectedDate = dt.Date; 
            GenerateTimeItems();

            IsHour = dt.Hour.ToString("00");
            IsMins = dt.Minute.ToString("00");
            IsSecs = dt.Second.ToString("00");

            // Update Text automatically
            Text = dt.ToString(GetValidFormat());
        }
        #endregion
    }
}
