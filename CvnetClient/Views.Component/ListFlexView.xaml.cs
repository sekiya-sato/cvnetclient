using CvnetClient.Class;
using CvnetClient.Models; 
using CvnetClient.ViewModels;
using System.Collections.ObjectModel;
using System.Data; 
using System.Windows;
using System.Windows.Controls; 

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for ListFlexView.xaml
    /// </summary>
    public partial class ListFlexView : UserControl
    {   
        private bool _isInitialized;

        public ListFlexView()
        {
            InitializeComponent();
            this.Loaded += ListFlexView_Loaded;  
            //Rows.CollectionChanged += (s, e) => GetQueryStr(); 
        }

        private void ListFlexView_Loaded(object sender, RoutedEventArgs e)
        { 
            if (!_isInitialized)
            {
                OnInit();
                _isInitialized = true;
            }
        }

        #region Dependency Property
        /*
        public ObservableCollection<ListFlexItem> DataSource
        {
            get => (ObservableCollection<ListFlexItem>)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(
                nameof(DataSource),
                typeof(ObservableCollection<ListFlexItem>),
                typeof(ListFlexView),
                new PropertyMetadata(null, OnRowsChanged)
        );

        public static readonly DependencyProperty ConfigProperty =
            DependencyProperty.Register(
            nameof(Config),
            typeof(ListFlexConfig),
            typeof(ListFlexView),
            //new PropertyMetadata(null));
            new PropertyMetadata(null, OnConfigChanged));

        public ListFlexConfig Config
        {
            get => (ListFlexConfig)GetValue(ConfigProperty);
            set => SetValue(ConfigProperty, value);
        }
        */

        public static readonly DependencyProperty DataSourceProperty =
            DependencyProperty.Register(nameof(DataSource),
                typeof(ListFlexData),
                typeof(ListFlexView),
                new PropertyMetadata(null)); 
        public ListFlexData DataSource
        {
            get => (ListFlexData)GetValue(DataSourceProperty);
            set => SetValue(DataSourceProperty, value);
        }
        #endregion

        #region Event List  
    /*
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

                control.GetQueryStr();  
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

            GetQueryStr(); // refresh when rows added/removed 
        }

        /// <summary>
        /// Trigger when edit a value in an existing row
        /// </summary>
        private void Row_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            GetQueryStr(); 
        }
    */

        private static void OnConfigChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ListFlexView view && e.NewValue is ListFlexConfig config)
            { 
                // If control already initialized, refresh immediately
                if (view._isInitialized)
                { 
                    view.OnInit(); 
                }
            }
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox combo && combo.DataContext is ListFlexItem row)
            {
                var selected = combo.SelectedValue as string;
                string[] rtn = DataSource.GetMaxStr(selected);
                var find_row = DataSource.ListFlexVar?.Unit_list.FirstOrDefault(x => x.Value == selected).Key;
                if (string.IsNullOrEmpty(find_row) || (find_row == selected))
                {
                    row.IsListFromEnabled = false;
                    row.IsListToEnabled = false;
                }
                else 
                {  
                    row.IsListFromEnabled = true;
                    row.IsListToEnabled = true;
                }
                row.FromValue = rtn[0];
                row.ToValue = rtn[1];
            }
        }
        #endregion

        #region Functions
        private void OnInit()
        {
            if (DataSource == null) return;

            // Ensure DataSource Rows exists
            if (DataSource.ListFlexSource == null)
                DataSource.ListFlexSource = new ObservableCollection<ListFlexItem>(); 
            else DataSource.ListFlexSource.Clear();

            if (DataSource == null) DataSource = new ListFlexData();
            else
            {
                var ListFlexVar = (DataSource.ListFlexVar != null) ? DataSource.ListFlexVar : DataSource.ListFlexVar = new ListFlexVariable();
                if (ListFlexVar.Type_Def == null || ListFlexVar.Type_Def.Count == 0)
                    ListFlexVar.Type_Def = new ObservableCollection<TypeDefItem>();
                else ListFlexVar.Type_Def.Clear();

                if (ListFlexVar.Unit_list == null || ListFlexVar.Unit_list.Count == 0)
                    ListFlexVar.Unit_list = new Dictionary<string, string>();
                else ListFlexVar.Unit_list.Clear();

                if (ListFlexVar.Unit_list2 == null || ListFlexVar.Unit_list2.Count == 0)
                    ListFlexVar.Unit_list2 = new Dictionary<string, string>();
                else ListFlexVar.Unit_list2.Clear();

                if (!string.IsNullOrEmpty(ListFlexVar.Line3_ListData))
                    ListFlexVar.Line3_ListData = string.Empty;

                //if (FlexData.ListFlexPara == null || FlexData.ListFlexPara.Count == 0)
                //    FlexData.ListFlexPara = new BizArray();
                //else FlexData.ListFlexPara.Clear();
            }
              
            List<string> v_col = new List<string>();
            string tb_name = "";
            string qs = "select 名称CD, 名称 from hc$master_meisho where 名称区分 = 'IDX' ";
            if (DataSource.ListConfig.flag == 0)
            {
                if (AppData.ClassCvnet.config.oroshi != 0)
                    qs += " and (名称CD between 'B01' and 'B18') ";
                else
                    qs += " and (名称CD between 'B01' and 'B10' or 名称CD between 'Y01' and 'Y20' or 名称CD in ('BRD','ITM','DZN','SZN','MKR','TNJ','SZI','GEN'" + ((AppData.ClassCvnet.config.smtflg == 1) ? " ,'BN0','BN1','BN2'" : "") + "))";

                if (AppData.ClassEtc.SearchShohinCols != null)
                {
                    for (var i = 0; i < AppData.ClassEtc.SearchShohinCols.Length; i++) {
                        v_col.Add(AppData.ClassEtc.SearchShohinCols[i]);
                    }
                }
                tb_name = "hc$master_shohin";
            }
            else if (DataSource.ListConfig.flag == 1)
            {
                qs += " and (名称CD between 'C01' and 'C10')";
                if (AppData.ClassEtc.SearchTokuiCols != null) {
                    for (var i = 0; i < AppData.ClassEtc.SearchTokuiCols.Length; i++) {
                        v_col.Add(AppData.ClassEtc.SearchTokuiCols[i]);
                    }
                }
                tb_name = "hc$master_tokui";
            }
            else if (DataSource.ListConfig.flag == 2)
            {
                qs += " and (名称CD between 'D01' and 'D10')";
                if (AppData.ClassEtc.SearchSiireCols != null) {
                    for (var i = 0; i < AppData.ClassEtc.SearchSiireCols.Length; i++) {
                        v_col.Add(AppData.ClassEtc.SearchSiireCols[i]);
                    }
                }
                tb_name = "hc$master_siire";
            }
            else if (DataSource.ListConfig.flag == 3)
            {
                qs += " and (名称CD between 'E01' and 'E05')";
                if (AppData.ClassEtc.SearchShainCols != null) {
                    for (var i = 0; i < AppData.ClassEtc.SearchShainCols.Length; i++) {
                        v_col.Add(AppData.ClassEtc.SearchShainCols[i]);
                    }
                }
                tb_name = "hc$master_shain";
            }
            qs += " and ランク > '0' order by 名称CD";
            var ret_csv = AppData.Http?.AspxSqlQuery(qs);
            foreach (DataRow row in ret_csv.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                DataSource.ListFlexVar?.Unit_list.Add(key, value);
            }

            if (DataSource.ListConfig.flag == 1)
            {
                DataSource.ListFlexVar?.Unit_list.Add("営業担当CD", "営業担当CD"); //unit_list.Add("", "営業担当CD");
                DataSource.ListFlexVar?.Unit_list.Add("請求先CD", "請求先CD"); //unit_list.Add("", "請求先CD");
            }
            else if (DataSource.ListConfig.flag == 2)
                DataSource.ListFlexVar?.Unit_list.Add("支払先CD", "支払先CD"); //unit_list.Add("", "支払先CD");

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
                DataSource.ListFlexVar.Type_Def = new ObservableCollection<TypeDefItem>(list);
            }

            DataSource.ListFlexVar.Line3_ListData = "";
            foreach (DataRow row in ret_csv.Rows)
            {
                if (!(AppData.ClassSatoo.LoginKubun == 1 && row[1].ToString() == "仕入先"))
                {
                    if (AppData.DefConfig.g_name.ContainsKey(row[1].ToString()))
                        DataSource.ListFlexVar.Line3_ListData += ((DataSource.ListFlexVar.Line3_ListData == "") ? "" : ",") + AppData.DefConfig.g_name[row[1].ToString()];
                }
            }

            if (v_col.Count > 0)
            {
                for (int i = 0; i < v_col.Count; i++)
                {
                    if (!(!AppData.DefConfig.g_name.ContainsKey(v_col[i]) || AppData.DefConfig.g_name[v_col[i]] == ""))
                    {
                        DataSource.ListFlexVar.Unit_list2.Add(AppData.DefConfig.g_name[v_col[i]], v_col[i]);
                        DataSource.ListFlexVar.Line3_ListData += "," + AppData.DefConfig.g_name[v_col[i]];
                    }
                    else DataSource.ListFlexVar.Unit_list2.Add(v_col[i], v_col[i]); 
                }
            }

            List<CsvItem> init_para = new List<CsvItem>();
            if (DataSource.ListConfig.init_csv == null || DataSource.ListConfig.init_csv?.Count == 0)
            {
                init_para = AppData.ClassEtc.ListFlexPara ?? new List<CsvItem>();
                switch (DataSource.ListConfig.flag)
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
            else init_para = DataSource.ListConfig.init_csv;

            for (int i = 0; i < init_para.Count; i++)
            {
                string find_row = (DataSource.ListFlexVar.Unit_list2.ContainsValue(init_para[i].col01)) ? init_para[i].col01 : string.Empty;
                var col = "";
                if (string.IsNullOrEmpty(find_row))
                {
                    col = init_para[i].col01;
                    string find_row2 = (DataSource.ListFlexVar.Unit_list.ContainsValue(init_para[i].col01)) ? init_para[i].col01 : string.Empty;
                    if (string.IsNullOrEmpty(find_row2)) continue;
                }
                else {
                    col = DataSource.ListFlexVar.Unit_list2[find_row];
                }
                 
                var row = new ListFlexItem
                {
                    No = DataSource.ListFlexSource.Count + 1,
                    SelectedItem = col
                };
                string[] rtn = DataSource.GetMaxStr(col);
                if (init_para[i].col02 == string.Empty)
                    row.FromValue = rtn[0];
                else
                    row.FromValue = init_para[i].col02;
                if (init_para[i].col03 == string.Empty)
                    row.ToValue = rtn[1];
                else
                    row.ToValue = init_para[i].col03;
                /* unit_listに無い場合、ボタンを使用不可にする */
                find_row = DataSource.ListFlexVar.Unit_list.FirstOrDefault(x => x.Value == col).Key;
                if (string.IsNullOrEmpty(find_row))
                {
                    row.IsListFromEnabled = false;
                    row.IsListToEnabled = false;
                    //Add to unit_list 
                    DataSource.ListFlexVar.Unit_list.Add(col, col);
                }
                else
                {
                    row.IsListFromEnabled = true;
                    row.IsListToEnabled = true; 
                }

                /* 仕入先ログイン時の縛り処理 */
                if (AppData.ClassSatoo.LoginKubun == 1 && col == "仕入先")
                { 
                    row.FromValue = AppData.ClassSatoo.SHAIN_CD + " " + AppData.ClassSatoo.SHAIN_Name;
                    row.ToValue = AppData.ClassSatoo.SHAIN_CD + " " + AppData.ClassSatoo.SHAIN_Name;
                    row.IsListFromEnabled = false;
                    row.IsListToEnabled = false;
                }
                DataSource.ListFlexSource.Add(row);
            }
        }    
        #endregion

        #region Button Event
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            int nextNo = DataSource.ListFlexSource.Count + 1;
            DataSource.ListFlexSource.Add(new ListFlexItem
            {
                No = nextNo,
                SelectedItem = string.Empty, // default selection
                FromValue = string.Empty,
                ToValue = string.Empty,
                IsListFromEnabled = true,
                IsListToEnabled = true,
            }); 
        }

        private void ListFrom_Click(object sender, RoutedEventArgs e)
        { 
            if (sender is Button btn && btn.DataContext is ListFlexItem row)
            {
                //MessageBox.Show($"List From clicked for row {row.No}");
                if (string.IsNullOrEmpty(row.SelectedItem)) return;
                var wrk_para = DataSource.Search_Data(row.SelectedItem);
                if (wrk_para == null) return;
                wrk_para.Set(2, AppData.ClassSatoo.GetStringFirst(row.FromValue)); 
                var get_sel00 = AppData.DlgService.Get80gphSel(wrk_para.ToArray());
                if (get_sel00 != null && get_sel00.Select80gphItem != null)
                {
                    row.FromValue = get_sel00.Select80gphItem.Code;
                    row.ToValue = get_sel00.Select80gphItem.Code;
                }
            }
        }

        private void ListTo_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ListFlexItem row)
            {
                if (string.IsNullOrEmpty(row.SelectedItem)) return;
                var wrk_para = DataSource.Search_Data(row.SelectedItem);
                if (wrk_para == null) return;
                wrk_para.Set(2, AppData.ClassSatoo.GetStringFirst(row.ToValue));
                var get_sel00 = AppData.DlgService.Get80gphSel(wrk_para.ToArray());
                if (get_sel00 != null && get_sel00.Select80gphItem != null)
                { 
                    row.ToValue = get_sel00.Select80gphItem.Code;
                }
            }
        }

        private void DeleteRow_Click(object sender, RoutedEventArgs e)
        {
            if (sender is Button btn && btn.DataContext is ListFlexItem row)
            {
                DataSource.ListFlexSource.Remove(row);

                // re-number No after delete
                for (int i = 0; i < DataSource.ListFlexSource.Count; i++)
                {
                    DataSource.ListFlexSource[i].No = i + 1;
                }
            }
        }
        #endregion 
    }
}