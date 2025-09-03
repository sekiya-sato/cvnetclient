using CvnetClient.Models;
using CvnetClient.ViewModels.Component;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
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
        private Dictionary<string, string> unit_list2;
        private string Line3_ListData;

        public ListFlexView()
        {
            InitializeComponent();
            Type_Def = new ObservableCollection<TypeDefItem>();
            unit_list2 = new Dictionary<string, string>(); 
            Rows = new ObservableCollection<ListFlexItem>();
            Rows.CollectionChanged += (s, e) => UpdateQuery(); 
        }

        #region Dependency Property
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
                new PropertyMetadata(null,OnRowsChanged)
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
        public static readonly DependencyProperty UnitListProperty =
            DependencyProperty.Register(nameof(unit_list),
                typeof(Dictionary<string,string>),
                typeof(ListFlexView),
                new PropertyMetadata(null)); 
        public Dictionary<string, string> unit_list
        {
            get => (Dictionary<string, string>)GetValue(UnitListProperty);
            set => SetValue(UnitListProperty, value);
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
        private static void OnRowsChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListFlexView control)
            {
                if (e.OldValue is ObservableCollection<ListFlexItem> oldRows)
                {
                    oldRows.CollectionChanged -= control.Rows_CollectionChanged;
                    foreach (var r in oldRows)
                        r.PropertyChanged -= control.Row_PropertyChanged;
                }

                if (e.NewValue is ObservableCollection<ListFlexItem> newRows)
                {
                    newRows.CollectionChanged += control.Rows_CollectionChanged;
                    foreach (var r in newRows)
                        r.PropertyChanged += control.Row_PropertyChanged;
                }

                control.UpdateQuery();
            }
        }

        /// <summary>
        /// Trigger when delete a row / add a new row in the DataGrid
        /// </summary>
        private void Rows_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ListFlexItem item in e.NewItems)
                    item.PropertyChanged += Row_PropertyChanged;
            }
            if (e.OldItems != null)
            {
                foreach (ListFlexItem item in e.OldItems)
                    item.PropertyChanged -= Row_PropertyChanged;
            }

            UpdateQuery(); // refresh when rows added/removed
        }

        /// <summary>
        /// Trigger when edit a value in an existing row
        /// </summary>
        private void Row_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            UpdateQuery();
        }

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
            unit_list = new Dictionary<string, string>();

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
            var ret_csv = AppData.Http?.AspxSqlQuery(qs);
            foreach (DataRow row in ret_csv.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                unit_list.Add(key, value);
            }

            if (config.flag == 1)
            {
                unit_list.Add("", "営業担当CD");
                unit_list.Add("", "請求先CD");
            }
            else if (config.flag == 2) 
                unit_list.Add("", "支払先CD"); 

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

            Line3_ListData = "";
            foreach (DataRow row in ret_csv.Rows)
            {
                if (!(ClassSatoo.LoginKubun == 1 && row[1].ToString() == "仕入先"))
                {
                    if (AppData.DefConfig.g_name.ContainsKey(row[1].ToString()))
                        Line3_ListData += ((Line3_ListData == "") ? "" : ",") + AppData.DefConfig.g_name[row[1].ToString()];
                }
            }

            if (v_col.Count > 0)
            {
                for (int i = 0; i < v_col.Count; i++)
                {
                    if (!(!AppData.DefConfig.g_name.ContainsKey(v_col[i]) || AppData.DefConfig.g_name[v_col[i]] == ""))
                    {
                        unit_list2.Add(AppData.DefConfig.g_name[v_col[i]], v_col[i]);
                        Line3_ListData += "," + AppData.DefConfig.g_name[v_col[i]];
                    }
                    else unit_list2.Add(v_col[i], v_col[i]); 
                }
            }

            List<CsvItem> init_para = new List<CsvItem>();
            if (config.init_csv == null || config.init_csv?.Count == 0)
            {
                init_para = AppData.ClassEtc.ListFlexPara ?? new List<CsvItem>();
                switch (config.flag)
                {
                    case 0:
                        init_para = AppData.ClassEtc.Bunrui_List0 ?? new List<CsvItem>();
                        break;
                    case 1:
                        init_para = AppData.ClassEtc.Bunrui_List1 ?? new List<CsvItem>();
                        break;
                    case 2:
                        init_para = AppData.ClassEtc.Bunrui_List2 ?? new List<CsvItem>();
                        break;
                    case 3:
                        init_para = AppData.ClassEtc.Bunrui_List3 ?? new List<CsvItem>();
                        break;
                }
            }
            else init_para = config.init_csv;

            for (int i = 0; i < init_para.Count; i++)
            {
                string find_row = (unit_list2.ContainsValue(init_para[i].col01)) ? init_para[i].col01 : string.Empty;
                var col = "";
                if (string.IsNullOrEmpty(find_row))
                {
                    col = init_para[i].col01;
                    string find_row2 = (unit_list.ContainsValue(init_para[i].col01)) ? init_para[i].col01 : string.Empty;
                    if (string.IsNullOrEmpty(find_row2)) continue;
                }
                else {
                    col = unit_list2[find_row];
                }

                Rows.Add(new ListFlexItem
                {
                    No = Rows.Count + 1,
                    SelectedItem = col 
                });
            }
        }

        private string GetConvKubun(string kubun)
        { 
            string cd_name = string.Empty;
            if (kubun == "ITM") cd_name = "アイテムCD";
            else if (kubun == "BRD") cd_name = "ブランドCD";
            else if (kubun == "DZN") cd_name = "デザイナーCD";
            else if (kubun == "SZN") cd_name = "シーズンCD";
            else if (kubun == "TNJ") cd_name = "展示会CD";
            else if (kubun == "SZI") cd_name = "素材CD";
            else if (kubun == "MKR") cd_name = "メーカーCD";
            else if (kubun == "GEN") cd_name = "原産国CD";
            else if (kubun == "BN0") cd_name = "大分類CD";
            else if (kubun == "BN1") cd_name = "中分類CD";
            else if (kubun == "BN2") cd_name = "小分類CD";
            else 
            {
                if (Regex.IsMatch(kubun, @"^B[0-9]{2}$")) {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
                else if (Regex.IsMatch(kubun, @"^C[0-9]{2}$")) {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
                else if (Regex.IsMatch(kubun, @"^D[0-9]{2}$")) {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
                else if (Regex.IsMatch(kubun, @"^E[0-9]{2}$")) {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
            }
            return cd_name;
        }

        private string[] Search_Data(string mst_name)
        {
            string[] v_ar = new string[3];
            var find_row = unit_list.FirstOrDefault(x => x.Value == mst_name).Key;
            var find_row2 = unit_list2.FirstOrDefault(x => x.Value == mst_name).Key;

            if (string.IsNullOrEmpty(find_row))
            { 
                v_ar[1] = mst_name;
                if (string.IsNullOrEmpty(find_row2))
                    v_ar = null;
                else
                    v_ar[1] = mst_name;
                return v_ar;
            }
            v_ar[1] = unit_list[find_row].Trim();
            v_ar[0] = "HC$MASTER_MEISHO";

            return v_ar;
        }

        private void GetMaxStr(string col)
        { 
            
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
