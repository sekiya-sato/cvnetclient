using CvnetClient.ViewModels.Component;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace CvnetClient.Views.Component
{
    /// <summary>
    /// Interaction logic for ListFlexView.xaml
    /// </summary>
    public partial class ListFlexView : UserControl
    {  
        public ListFlexView()
        {
            InitializeComponent();
            Rows = new ObservableCollection<ListFlexItem>(); 
        } 

        public ObservableCollection<ListFlexItem> Rows
        {
            get => (ObservableCollection<ListFlexItem>)GetValue(RowsProperty);
            set => SetValue(RowsProperty, value);
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(ObservableCollection<ListFlexItem>),
                typeof(ListFlexView),
                new PropertyMetadata(new ObservableCollection<ListFlexItem>())
        );

        public double GridWidth
        {
            get => (double)GetValue(GridWidthProperty);
            set => SetValue(GridWidthProperty, value);
        }

        public static readonly DependencyProperty GridWidthProperty =
            DependencyProperty.Register(
                nameof(GridWidth),
                typeof(double),
                typeof(ListFlexView),
                new PropertyMetadata(double.NaN) // default: Auto
            );

        public double GridHeight
        {
            get => (double)GetValue(GridHeightProperty);
            set => SetValue(GridHeightProperty, value);
        }

        public static readonly DependencyProperty GridHeightProperty =
            DependencyProperty.Register(
                nameof(GridHeight),
                typeof(double),
                typeof(ListFlexView),
                new PropertyMetadata(double.NaN) // default: Auto
            );



        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            int nextNo = Rows.Count + 1; 
            Rows.Add(new ListFlexItem
            {
                No = nextNo,
                SelectedItem = "Item1", // default selection
                FromValue = string.Empty,
                ToValue = string.Empty
            });
        }

        private void ListFrom_Click(object sender, RoutedEventArgs e)
        {
            // Example: get the current row from DataContext
            if (sender is Button btn && btn.DataContext is ListFlexItem row)
            {
                //MessageBox.Show($"List From clicked for row {row.No}");
            }
        }

        private void ListTo_Click(object sender, RoutedEventArgs e)
        {

        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ListFlexItem row)
            {
                Rows.Remove(row);

                // re-number No after delete
                for (int i = 0; i < Rows.Count; i++)
                {
                    Rows[i].No = i + 1;
                }
            }
        }
    }
}
