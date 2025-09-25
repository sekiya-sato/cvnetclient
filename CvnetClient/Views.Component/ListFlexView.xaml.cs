using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.ViewModels;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace CvnetClient.Views
{
    /// <summary>
    /// Interaction logic for ListFlexView.xaml
    /// </summary>
    public partial class ListFlexView : UserControl
    { 
        private ObservableCollection<TypeDefItem> Type_Def;
        private Dictionary<string, string> unit_list2;
        private string Line3_ListData;
        private bool _isInitialized;

        public ListFlexView()
        {
            InitializeComponent();
            this.Loaded += ListFlexView_Loaded;
            Type_Def = new ObservableCollection<TypeDefItem>();
            unit_list2 = new Dictionary<string, string>(); 
            //Rows = new ObservableCollection<ListFlexItem>();
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
                //new PropertyMetadata(null));
                new PropertyMetadata(null, OnConfigChanged));

        public ListFlexConfig Config
        { 
            get => (ListFlexConfig)GetValue(ConfigProperty);
            set => SetValue(ConfigProperty, value);
        }

        // Generated SQL-like WHERE string (For example: item1 between :1 and :2)
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

        // Query String parameter 
        public static readonly DependencyProperty QueryStrParaProperty =
            DependencyProperty.Register(
                nameof(QueryStrPara),
                typeof(BizArray),
                typeof(ListFlexView),
                new PropertyMetadata(new BizArray()));

        public BizArray QueryStrPara
        {
            get => (BizArray)GetValue(QueryStrParaProperty);
            set => SetValue(QueryStrParaProperty, value);
        }

        // Generate SQL-like WHERE string format2 (For example: item1 between '' and 'zzzzzz')
        public static readonly DependencyProperty QueryString2Property =
            DependencyProperty.Register(
                nameof(QueryString2),
                typeof(string),
                typeof(ListFlexView),
                new FrameworkPropertyMetadata(
                    string.Empty,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));
        public string QueryString2
        {
            get => (string)GetValue(QueryString2Property);
            set => SetValue(QueryString2Property, value);
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
                string[] rtn = GetMaxStr(selected);
                var find_row = unit_list.FirstOrDefault(x => x.Value == selected).Key;
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
            if (Config == null) return;

            // Ensure Rows exists
            if (Rows == null) 
                Rows = new ObservableCollection<ListFlexItem>(); 
            else Rows.Clear(); 
              
            if (unit_list == null || unit_list.Count == 0)
                unit_list = new Dictionary<string, string>();
            else unit_list.Clear();
            if (unit_list2 == null || unit_list2.Count == 0)
                unit_list2 = new Dictionary<string, string>();
            else unit_list2.Clear();
            if (Type_Def == null || Type_Def.Count == 0)
                Type_Def = new ObservableCollection<TypeDefItem>();
            else Type_Def.Clear(); 
            if (QueryStrPara == null || QueryStrPara.Count == 0)
                QueryStrPara = new BizArray();
            else QueryStrPara.Clear();

            Line3_ListData = ""; 
            List<string> v_col = new List<string>();
            string tb_name = "";
            string qs = "select 名称CD, 名称 from hc$master_meisho where 名称区分 = 'IDX' ";
            if (Config.flag == 0)
            {
                if (AppData.ClassCvnet.config.oroshi != 0)
                    qs += " and (名称CD between 'B01' and 'B18') ";
                else
                    qs += " and (名称CD between 'B01' and 'B10' or 名称CD between 'Y01' and 'Y20' or 名称CD in ('BRD','ITM','DZN','SZN','MKR','TNJ','SZI','GEN'" + ((AppData.ClassCvnet.config.smtflg == 1) ? " ,'BN0','BN1','BN2'" : "") + "))";

                for (var i = 0; i < AppData.ClassEtc.SearchShohinCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchShohinCols[i]);
                }
                tb_name = "hc$master_shohin";
            }
            else if (Config.flag == 1)
            {
                qs += " and (名称CD between 'C01' and 'C10')";
                for (var i = 0; i < AppData.ClassEtc.SearchTokuiCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchTokuiCols[i]);
                }
                tb_name = "hc$master_tokui";
            }
            else if (Config.flag == 2)
            {
                qs += " and (名称CD between 'D01' and 'D10')";
                for (var i = 0; i < AppData.ClassEtc.SearchSiireCols.Length; i++) {
                    v_col.Add(AppData.ClassEtc.SearchSiireCols[i]);
                }
                tb_name = "hc$master_siire";
            }
            else if (Config.flag == 3)
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

            if (Config.flag == 1)
            { 
                unit_list.Add("営業担当CD", "営業担当CD"); //unit_list.Add("", "営業担当CD");
                unit_list.Add("請求先CD", "請求先CD"); //unit_list.Add("", "請求先CD");
            }
            else if (Config.flag == 2) 
                unit_list.Add("支払先CD", "支払先CD"); //unit_list.Add("", "支払先CD");

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
                if (!(AppData.ClassSatoo.LoginKubun == 1 && row[1].ToString() == "仕入先"))
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
            if (Config.init_csv == null || Config.init_csv?.Count == 0)
            {
                init_para = AppData.ClassEtc.ListFlexPara ?? new List<CsvItem>();
                switch (Config.flag)
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
            else init_para = Config.init_csv;

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
                 
                var row = new ListFlexItem
                {
                    No = Rows.Count + 1,
                    SelectedItem = col
                };
                string[] rtn = GetMaxStr(col);
                if (init_para[i].col02 == string.Empty)
                    row.FromValue = rtn[0];
                else
                    row.FromValue = init_para[i].col02;
                if (init_para[i].col03 == string.Empty)
                    row.ToValue = rtn[1];
                else
                    row.ToValue = init_para[i].col03;
                /* unit_listに無い場合、ボタンを使用不可にする */
                find_row = unit_list.FirstOrDefault(x => x.Value == col).Key;
                if (string.IsNullOrEmpty(find_row))
                {
                    row.IsListFromEnabled = false;
                    row.IsListToEnabled = false; 
                    //Add to unit_list 
                    unit_list.Add(col, col);
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
                Rows.Add(row);
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
        private string GetConvColName(string col_name)
        {
            var find_row = unit_list2.FirstOrDefault(x => x.Value == col_name).Key;
            if (string.IsNullOrEmpty(find_row))
                return string.Empty;
            return unit_list2[find_row];
        }
        private BizArray Search_Data(string mst_name)
        {
            var v_ar = new BizArray();
            var find_row = unit_list.FirstOrDefault(x => x.Value == mst_name).Key;
            var find_row2 = unit_list2.FirstOrDefault(x => x.Value == mst_name).Key;

            if (string.IsNullOrEmpty(find_row))
            {  
                v_ar.Set(1, mst_name);
                if (string.IsNullOrEmpty(find_row2))
                    v_ar = null;
                else
                    v_ar.Set(1, mst_name);
                return v_ar;
            } 
            v_ar.Set(0, "HC$MASTER_MEISHO");
            v_ar.Set(1, find_row); 
            if (mst_name == "商品CD") {  
                v_ar.Set(0, "HC$MASTER_SHOHIN");
                v_ar.Set(1, find_row);
            }
            if (mst_name == "得意先CD")
            {
                v_ar.Set(0, "HC$MASTER_TOKUI");
                v_ar.Set(1, find_row);
            }

            /* 2011.03.04 追加 */
            if (mst_name == "営業担当CD")
            { 
                v_ar.Set(0, "HC$MASTER_SHAIN");
                v_ar.Set(1, find_row);
            }
            if (mst_name == "請求先CD")
            {
                v_ar.Set(0, "HC$MASTER_TOKUI");
                v_ar.Set(1, find_row);
                v_ar.Set(3, " AND 得意先CD=請求先CD OR 請求先CD='.' ");
            }
            if (mst_name == "支払先CD")
            {
                v_ar.Set(0, "HC$MASTER_SIIRE");
                v_ar.Set(1, find_row);
                v_ar.Set(3, " AND 仕入先CD=支払先CD or 支払先CD='.' ");
            } 
            return v_ar;
        }

        private string[] GetMaxStr(string col)
        {
            var rt_ar = new BizArray();

            if (string.IsNullOrEmpty(col))
            {
                rt_ar.Set(0, "");
                rt_ar.Set(1, "zzzzzzzzzzzzzzzzzzzz");
                return rt_ar.ToArray();
            }
             
            var ar = Search_Data(col);
            string cd_name = GetConvKubun(ar[1]);
            if (cd_name == "")
                cd_name =  GetConvColName(col);
            var find_row = Type_Def.Where(x => x.column_name == cd_name).FirstOrDefault();
            if (find_row != null)
            { 
                rt_ar.Set(0, "");
                rt_ar.Set(1, "zzzzzzzzzzzzzzzzzzzz"); 
                return rt_ar.ToArray();
            }
            else 
            {
                if (find_row?.data_type.ToLower() == "number")
                {
                    rt_ar.Set(0, "0");
                    rt_ar.Set(1, "9999999999");
                    return rt_ar.ToArray();
                }
                else 
                { 
                    rt_ar.Set(0, "");
                    rt_ar.Set(1, "zzzzzzzzzzzzzzzzzzzz");
                    return rt_ar.ToArray();
                }
            }
        } 
        #endregion

        #region Button Event
        private void AddRow_Click(object sender, RoutedEventArgs e)
        {
            int nextNo = Rows.Count + 1; 
            Rows.Add(new ListFlexItem
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
                var wrk_para = Search_Data(row.SelectedItem);
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
                var wrk_para = Search_Data(row.SelectedItem);
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
        private void GetQueryStr()
        {
            if (Rows == null || Rows.Count == 0) return; 
            string conn_str = (Config.conn_flg == 0) ? " AND " : " OR ";

            int cnt = Config.cnt_start; 
            QueryString = string.Empty; QueryString2 = string.Empty; 
            QueryStrPara.Clear();

            foreach (var row in Rows)
            {
                if(string.IsNullOrEmpty(row.SelectedItem)) continue; 
                var ar = Search_Data(row.SelectedItem).ToArray();
                if (!string.IsNullOrEmpty(ar[1]))
                {
                    var cd_name = GetConvKubun(ar[1]);
                    if (string.IsNullOrEmpty(cd_name))
                        cd_name =  GetConvColName(row.SelectedItem);
                    if (!string.IsNullOrEmpty(cd_name))
                    {
                        if (Config.cnt_start < cnt)
                        {
                            QueryString += conn_str;
                            QueryString2 += conn_str;
                        }

                        string CD01 = (string.IsNullOrEmpty(row.FromValue.Trim())) ? "." : row.FromValue.Trim();
                        string CD02 = (string.IsNullOrEmpty(row.ToValue.Trim())) ? "." : row.ToValue.Trim();
                        QueryString2 += Config.col_alias + cd_name + " between '" + CD01 + "' and '" + CD02 + "'";

                        QueryString += Config.col_alias + cd_name + " between :" + cnt + " and :" + (cnt + 1);
                        cnt = cnt + 2;
                        QueryStrPara[QueryStrPara.Count] = row.FromValue;
                        QueryStrPara[QueryStrPara.Count] = row.ToValue;
                    }
                }
            }
            if (QueryString != "") { QueryString = " and (" + QueryString + ")"; }
            if (QueryString2 != "") { QueryString2 = " and (" + QueryString2 + ")"; }
        }
    }
}