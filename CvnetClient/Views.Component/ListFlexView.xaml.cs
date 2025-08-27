using CvnetClient.Models;
using CvnetClient.ViewModels.Component;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace CvnetClient.Views.Component
{
    /// <summary>
    /// Interaction logic for ListFlexView.xaml
    /// </summary>
    public partial class ListFlexView : UserControl
    {
        private ObservableCollection<TypeDefItem> Type_Def;

        public ListFlexView()
        {
            InitializeComponent();
            Type_Def = new ObservableCollection<TypeDefItem>();
            Rows = new ObservableCollection<ListFlexItem>();
            Rows.CollectionChanged += (s, e) => UpdateQuery(); 
        }

        #region Dependency Property
        public ObservableCollection<ListFlexItem> Rows
        {
            get => (ObservableCollection<ListFlexItem>)GetValue(RowsProperty);
            set
            {
                SetValue(RowsProperty, value);
                if (value != null)
                {
                    foreach (var r in value)
                        r.PropertyChanged += (s, e) => UpdateQuery();
                }
                UpdateQuery();
            }
        }

        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register(
                nameof(Rows),
                typeof(ObservableCollection<ListFlexItem>),
                typeof(ListFlexView),
                new PropertyMetadata(new ObservableCollection<ListFlexItem>())
        ); 

        // DataGrid size
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

        // DataGrid size
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

        // ComboBox Items
        public static readonly DependencyProperty SearchItemsProperty =
            DependencyProperty.Register(nameof(SearchItems),
                typeof(Dictionary<string,string>),
                typeof(ListFlexView),
                new PropertyMetadata(null)); 
        public Dictionary<string, string> SearchItems
        {
            get => (Dictionary<string, string>)GetValue(SearchItemsProperty);
            set => SetValue(SearchItemsProperty, value);
        }

        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register(
                nameof(Config),
                typeof(ListFlexConfig),
                typeof(ListFlexView),
                new PropertyMetadata(null, OnConfigChanged));
        public ListFlexConfig Config
        { 
            get => (ListFlexConfig)GetValue(ConfigProperty);
            set => SetValue(ConfigProperty, value);
        }

        // Generated SQL-like WHERE string
        public static readonly DependencyProperty QueryStringProperty =
            DependencyProperty.Register(
                nameof(QueryString),
                typeof(string),
                typeof(ListFlexView),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

        public string QueryString
        {
            get => (string)GetValue(QueryStringProperty);
            set => SetValue(QueryStringProperty, value);
        } 
        #endregion

        #region Event List 
        private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListFlexView view && e.NewValue is ListFlexConfig config)
            {
                view.OnInit(config);
            }
        }
        private void OnInit(ListFlexConfig config)
        { 
            Rows.Clear(); 
            SearchItems = new Dictionary<string, string>();

            List<string> v_col = new List<string>();
            string tb_name = "";
            string qs = "select 名称CD, 名称 from hc$master_meisho where 名称区分 = 'IDX' ";
            if (config.flag == 0)
            {
                if (AppData.cvnet.config.oroshi != 0)
                    qs += " and (名称CD between 'B01' and 'B18') ";
                else
                    qs += " and (名称CD between 'B01' and 'B10' or 名称CD between 'Y01' and 'Y20' or 名称CD in ('BRD','ITM','DZN','SZN','MKR','TNJ','SZI','GEN'" + ((AppData.cvnet.config.smtflg == 1) ? " ,'BN0','BN1','BN2'" : "") + "))";

                for (var i = 0; i < AppData.ClassEtc.SearchShohinCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchShohinCols[i]);
                }
                tb_name = "hc$master_shohin";
            }
            else if (config.flag == 1)
            {
                qs += " and (名称CD between 'C01' and 'C10')";
                for (var i = 0; i < AppData.ClassEtc.SearchTokuiCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchTokuiCols[i]);
                }
                tb_name = "hc$master_tokui";
            }
            else if (config.flag == 2)
            {
                qs += " and (名称CD between 'D01' and 'D10')";
                for (var i = 0; i < AppData.ClassEtc.SearchSiireCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchSiireCols[i]);
                }
                tb_name = "hc$master_siire";
            }
            else if (config.flag == 3)
            {
                qs += " and (名称CD between 'E01' and 'E05')";
                for (var i = 0; i < AppData.ClassEtc.SearchShainCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchShainCols[i]);
                }
                tb_name = "hc$master_shain";
            }
            qs += " and ランク > '0' order by 名称CD";
            var unit_list = AppData.Http?.AspxSqlQuery(qs);
            foreach (DataRow row in unit_list.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                SearchItems.Add(key, string.Format("{0} {1}", key, value));
            }

            if (config.flag == 1)
            {
                SearchItems.Add("", "営業担当CD");
                SearchItems.Add("", "請求先CD");
            }
            else if (config.flag == 2) 
                SearchItems.Add("", "支払先CD"); 

            string sql = "select column_name, lower(data_type) as data_type from all_tab_columns where lower(table_name) = '" + tb_name + "'"
                       + " group by column_name,data_type order by max(column_id)";
            var ret_csv2 = AppData.Http?.AspxSqlQuery(sql);
            if (ret_csv2 != null && ret_csv2.Rows.Count > 0)
            {
                var list = (from DataRow dr in ret_csv2.Rows
                            select new TypeDefItem
                            {
                                column_name = dr["column_name"].ToString() ?? string.Empty,
                                data_type = dr["data_type"].ToString() ?? string.Empty
                            }).OrderBy(c => c.column_name).ToList();
                Type_Def = new ObservableCollection<TypeDefItem>(list);
            }
        }

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
        #endregion

        // ---------------------------
        // Generate SQL-like WHERE string
        // ---------------------------
        private void UpdateQuery()
        {
            if (Rows == null || Rows.Count == 0)
            {
                QueryString = string.Empty;
                return;
            }

            var parts = Rows
                .Where(r => !string.IsNullOrWhiteSpace(r.FromValue) || !string.IsNullOrWhiteSpace(r.ToValue))
                .Select(r =>
                    $"AND {Config.col_alias}{r.SelectedItem} BETWEEN \"{r.FromValue}\" AND \"{r.ToValue}\"");

            QueryString = string.Join(" \n", parts);
        }
    }
}
