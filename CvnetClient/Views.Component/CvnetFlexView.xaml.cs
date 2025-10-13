using System.Data;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for CvnetFlexView.xaml
    /// </summary>
    public partial class CvnetFlexView : DataGrid
    {
        public CvnetFlexView()
        {
            InitializeComponent();

            AutoGenerateColumns = false;
            CanUserAddRows = false;
            CanUserDeleteRows = false;
            SelectionMode = DataGridSelectionMode.Single;

            ContextMenu = CreateContextMenu();

            LoadingRow += CvnetFlexView_LoadingRow;
            PreviewKeyDown += CvnetFlexView_PreviewKeyDown;
            MouseRightButtonUp += CvnetFlexView_MouseRightButtonUp;
        }

        #region DataSource Bind & Changed
        public static readonly DependencyProperty DataSourceProperty =
                   DependencyProperty.Register(nameof(DataSource), typeof(DataTable), typeof(CvnetFlexView),
                       new PropertyMetadata(null, OnDataSourceChanged));

        public DataTable DataSource
        {
            get => (DataTable)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        private static void OnDataSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var grid = (CvnetFlexView)d;
            grid.ItemsSource = (e.NewValue as DataTable)?.DefaultView;

            grid.AddDeleteColumn();
        }
        #endregion

        #region Delete Features
        private void AddDeleteColumn()
        {
            // Check if already added
            foreach (var col in Columns)
            {
                if (col is DataGridTemplateColumn templateCol && templateCol.Header?.ToString() == "Delete")
                    return;
            }

            var deleteTemplate = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(Button));
            factory.SetValue(Button.ContentProperty, "✖");
            factory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.Red);
            factory.SetValue(Button.PaddingProperty, new Thickness(4));
            factory.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteButton_Click));
            factory.SetBinding(Button.CommandParameterProperty, new System.Windows.Data.Binding());

            deleteTemplate.VisualTree = factory;

            var deleteColumn = new DataGridTemplateColumn
            {
                Header = "Delete",
                CellTemplate = deleteTemplate
            };

            Columns.Add(deleteColumn);
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is DataRowView rowView)
            {
                DataSource?.Rows.Remove(rowView.Row);
            }
        }
        #endregion

        #region Events
        private void CvnetFlexView_LoadingRow(object? sender, DataGridRowEventArgs e)
        {
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();
        }
        private void CvnetFlexView_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Delete)
            {
                DeleteSelectedRow();
                e.Handled = true;
            }
        } 
        private void CvnetFlexView_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItem != null)
                ContextMenu.IsOpen = true;
        }
        #endregion

        #region Function
        private ContextMenu CreateContextMenu()
        {
            var menu = new ContextMenu();

            var addItem = new MenuItem { Header = "Add Row" };
            addItem.Click += (s, e) => AddRow();

            var deleteItem = new MenuItem { Header = "Delete Row" };
            deleteItem.Click += (s, e) => DeleteSelectedRow();

            menu.Items.Add(addItem);
            menu.Items.Add(deleteItem);

            return menu;
        }
        private void AddRow()
        {
            DataSource?.Rows.Add(DataSource.NewRow());
        }
        private void DeleteSelectedRow()
        {
            if (SelectedItem is DataRowView rowView)
            {
                DataSource.Rows.Remove(rowView.Row);
            }
        }
        #endregion
    }
}
