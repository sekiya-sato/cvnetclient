using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
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

        #region Index & Delete Features

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            base.OnItemsSourceChanged(oldValue, newValue);
            AddIndexColumn();
            AddDeleteColumn();   
        }

        private void AddIndexColumn()
        {
            // Check if "行" column already exists (regardless of column type)
            foreach (var col in Columns)
            {
                if (col.Header?.ToString() == "行")
                    return;
            }

            var textFactory = new FrameworkElementFactory(typeof(TextBlock));
            textFactory.SetBinding(TextBlock.TextProperty, new System.Windows.Data.Binding
            {
                RelativeSource = new RelativeSource(RelativeSourceMode.FindAncestor, typeof(DataGridRow), 1),
                Path = new PropertyPath("Header")
            });
            textFactory.SetValue(TextBlock.HorizontalAlignmentProperty, HorizontalAlignment.Center);
            textFactory.SetValue(TextBlock.VerticalAlignmentProperty, VerticalAlignment.Center);
            var cellTemplate = new DataTemplate();
            cellTemplate.VisualTree = textFactory;
            

            var indexColumn = new DataGridTemplateColumn
            {

                Header = new TextBlock
                {
                    Text = "行",
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                IsReadOnly = true,
                Width = 40,
                CellTemplate = cellTemplate,
                
            };

            Columns.Insert(0, indexColumn);
        }

        private void AddDeleteColumn()
        {
            // already added?
            foreach (var col in Columns)
            {
                if (col is DataGridTemplateColumn templateCol && templateCol.Header?.ToString() == "削除")
                    return;
            }
            
            var deleteTemplate = new DataTemplate();
            var factory = new FrameworkElementFactory(typeof(Button));
            factory.SetResourceReference(Button.StyleProperty, "DeleteFlexBtn");
            //factory.SetValue(Button.ContentProperty, "🗑");
            //factory.SetValue(Button.ForegroundProperty, System.Windows.Media.Brushes.Black);
            //factory.SetValue(Button.PaddingProperty, new Thickness(4));

            factory.AddHandler(Button.ClickEvent, new RoutedEventHandler(DeleteButton_Click));
            factory.SetBinding(Button.CommandParameterProperty, new System.Windows.Data.Binding());


            deleteTemplate.VisualTree = factory;
            
            var deleteColumn = new DataGridTemplateColumn
            {
                Header = new TextBlock
                {
                    Text = "削除",
                    FontWeight = FontWeights.Bold,
                    TextAlignment = TextAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                },
                CellTemplate = deleteTemplate,
                Width = 70,

            };

            Columns.Add(deleteColumn);
        }

        private void RenumberRows()
        {
            for (int i = 0; i < Items.Count; i++)
            {
                var row = ItemContainerGenerator.ContainerFromIndex(i) as DataGridRow;
                if (row != null)
                {
                    row.Header = (i + 1).ToString();
                }
            }
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.CommandParameter is object item)
            {
                if (ItemsSource is IList list)
                {
                    list.Remove(item);
                    RenumberRows();
                }
            }
        }
        #endregion

        #region Events
        private void CvnetFlexView_LoadingRow(object? sender, DataGridRowEventArgs e)
        { 
            // Show row number in row header (left gray bar)
            e.Row.Header = (e.Row.GetIndex() + 1).ToString();

            // Show row number inside the "No" column cell
            var index = e.Row.GetIndex() + 1;

            // Get the column 0 ("No")
            if (Columns.Count > 0 && Columns[0] is DataGridTextColumn)
            {
                var cellContent = Columns[0].GetCellContent(e.Row) as TextBlock;
                if (cellContent != null)
                {
                    cellContent.Text = index.ToString();
                }
            }
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

            var addItem = new MenuItem { Header = "➕ 最終行追加" };
            addItem.Click += (s, e) => AddRow();

            var deleteItem = new MenuItem { Header = "🗑 削除" };
            deleteItem.Click += (s, e) => DeleteSelectedRow();

            menu.Items.Add(addItem);
            menu.Items.Add(deleteItem);

            return menu;
        }
        private void AddRow()
        {
            if (ItemsSource is IList list)
            {
                var itemType = list.GetType().GetGenericArguments()[0];
                var newItem = Activator.CreateInstance(itemType);
                list.Add(newItem);
                RenumberRows();
            }
        }
        private void DeleteSelectedRow()
        {
            if (ItemsSource is IList list)
            {
                list.Remove(SelectedItem);
                RenumberRows();
            }
        }
        #endregion
    }
}
