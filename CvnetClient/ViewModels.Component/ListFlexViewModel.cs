using CommunityToolkit.Mvvm.ComponentModel;
using CvnetClient.Class;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace CvnetClient.ViewModels
{
    public class ListFlexData
    {
        public ObservableCollection<ListFlexItem>? ListFlexSource {  get; set; }

        public ListFlexConfig? ListConfig { get; set; }

        public ListFlexVariable? ListFlexVar { get; set; }

        //public string? WhereClaus {  get; set; }

        //public BizArray? ListFlexPara { get; set; }

        public ListFlexData()
        { 
            this.ListFlexSource = new ObservableCollection<ListFlexItem>();
            this.ListFlexVar = new ListFlexVariable();
            this.ListConfig = new ListFlexConfig();
            //this.WhereClaus = string.Empty;
            //this.ListFlexPara = new BizArray();
        }

        public string GetConvKubun(string kubun)
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
                if (Regex.IsMatch(kubun, @"^B[0-9]{2}$"))
                {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
                else if (Regex.IsMatch(kubun, @"^C[0-9]{2}$"))
                {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
                else if (Regex.IsMatch(kubun, @"^D[0-9]{2}$"))
                {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
                else if (Regex.IsMatch(kubun, @"^E[0-9]{2}$"))
                {
                    cd_name = "名称CD" + kubun.Substring(1, 2);
                }
            }
            return cd_name;
        }

        public string GetConvColName(string col_name)
        {
            var find_row = ListFlexVar.Unit_list2.FirstOrDefault(x => x.Value == col_name).Key;
            if (string.IsNullOrEmpty(find_row))
                return string.Empty;
            return ListFlexVar.Unit_list2[find_row];
        }

        public BizArray Search_Data(string mst_name)
        {
            var v_ar = new BizArray();
            var find_row = ListFlexVar.Unit_list.FirstOrDefault(x => x.Value == mst_name).Key;
            var find_row2 = ListFlexVar.Unit_list2.FirstOrDefault(x => x.Value == mst_name).Key;

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
            if (mst_name == "商品CD")
            {
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

        public string[] GetMaxStr(string col)
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
                cd_name = GetConvColName(col);
            var find_row = ListFlexVar.Type_Def.Where(x => x.column_name == cd_name).FirstOrDefault();
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

        // ---------------------------
        // Generate SQL-like WHERE string
        // --------------------------- 
        public string GetQueryStr(BizArray v_para = null, int? v_cnt = null, int? v_flg = null, string v_alias = "")
        {
            int? f_cnt = 0; 
            if (v_cnt == null)
            {
                f_cnt = ListConfig.cnt_start;
                v_cnt = ListConfig.cnt_start;
            }
            else f_cnt = v_cnt;

            int? flg = (v_flg == null) ? ListConfig.conn_flg : v_flg;
            string alias = (string.IsNullOrEmpty(v_alias)) ? ListConfig.col_alias : v_alias;

            string QueryString = string.Empty;

            if (ListFlexSource == null || ListFlexSource.Count == 0) return QueryString;
            string conn_str = (flg == 0) ? " AND " : " OR ";
            
            if (v_para == null) v_para = new BizArray(); 

            foreach (var row in ListFlexSource)
            {
                if (string.IsNullOrEmpty(row.SelectedItem)) continue;
                var ar = Search_Data(row.SelectedItem).ToArray();
                if (!string.IsNullOrEmpty(ar[1]))
                {
                    var cd_name = GetConvKubun(ar[1]);
                    if (string.IsNullOrEmpty(cd_name))
                        cd_name = GetConvColName(row.SelectedItem);
                    if (!string.IsNullOrEmpty(cd_name))
                    {
                        if (f_cnt < v_cnt)
                        {
                            QueryString += conn_str; 
                        } 

                        QueryString += alias + cd_name + " between :" + v_cnt + " and :" + (v_cnt + 1);
                        v_cnt = v_cnt + 2;
                        v_para[v_para.Count] = row.FromValue;
                        v_para[v_para.Count] = row.ToValue;
                    }
                }
            }
            if (QueryString != "") { QueryString = " and (" + QueryString + ")"; }
            return QueryString;
        }
        public string GetQueryStr2(BizArray v_para = null, int? v_cnt = null, int? v_flg = null, string v_alias = "")
        {
            int? f_cnt = 0;
            if (v_cnt == null)
            {
                f_cnt = ListConfig.cnt_start;
                v_cnt = ListConfig.cnt_start;
            }
            else f_cnt = v_cnt;
            int? flg = (v_flg == null) ? ListConfig.conn_flg : v_flg;
            string alias = (string.IsNullOrEmpty(v_alias)) ? ListConfig.col_alias : v_alias;
             
            string QueryString2 = string.Empty;

            if (ListFlexSource == null || ListFlexSource.Count == 0) return QueryString2;
            string conn_str = (flg == 0) ? " AND " : " OR ";

            if (v_para == null) v_para = new BizArray(); 

            foreach (var row in ListFlexSource)
            {
                if (string.IsNullOrEmpty(row.SelectedItem)) continue;
                var ar = Search_Data(row.SelectedItem).ToArray();
                if (!string.IsNullOrEmpty(ar[1]))
                {
                    var cd_name = GetConvKubun(ar[1]);
                    if (string.IsNullOrEmpty(cd_name))
                        cd_name = GetConvColName(row.SelectedItem);
                    if (!string.IsNullOrEmpty(cd_name))
                    {
                        if (f_cnt < v_cnt)
                        { 
                            QueryString2 += conn_str;
                        }

                        string CD01 = (string.IsNullOrEmpty(row.FromValue.Trim())) ? "." : row.FromValue.Trim();
                        string CD02 = (string.IsNullOrEmpty(row.ToValue.Trim())) ? "." : row.ToValue.Trim();
                        QueryString2 += alias + cd_name + " between '" + CD01 + "' and '" + CD02 + "'"; 
                        v_cnt = v_cnt + 2;
                        v_para[v_para.Count] = row.FromValue;
                        v_para[v_para.Count] = row.ToValue;
                    }
                }
            } 
            if (QueryString2 != "") { QueryString2 = " and (" + QueryString2 + ")"; }
            return QueryString2;
        }
    }

    public partial class ListFlexVariable : ObservableObject
    {
        [ObservableProperty]
        ObservableCollection<TypeDefItem> type_Def;

        [ObservableProperty]
        Dictionary<string, string> unit_list;

        [ObservableProperty]
        Dictionary<string, string> unit_list2;

        [ObservableProperty]
        string line3_ListData;

        public ListFlexVariable()
        { 
            this.Type_Def = new ObservableCollection<TypeDefItem>();
            this.Unit_list = new Dictionary<string, string>();
            this.Unit_list2 = new Dictionary<string, string>();
            this.Line3_ListData = string.Empty;
        }
    }

    /// <summary>
    /// Configuration for ListFlexView behavior
    /// </summary>
    public class ListFlexConfig
    { 
        public int flag { get; set; } = 0;

        /// <summary>
        /// Paramter Count Start [Example set cnt_start = 0, then query parameter start from :0]
        /// </summary>
        public int cnt_start { get; set; } = 0;

        /// <summary>
        /// 項目 Alias [Example set col_alias = "A.", then each 項目 will be like A.項目1, A.項目2]
        /// </summary>
        public string col_alias { get; set; } = string.Empty;
        
        /// <summary>
        /// Initial list of values for ComboBox
        /// </summary>
        public List<CsvItem> init_csv {  get; set; } = new List<CsvItem>();

        /// <summary>
        /// GetQueryStr() Condition flag 
        /// [Example set conn_flg = "0", then each 項目 will be like AND A.項目1 BETWEEN :1 AND 2: ]
        /// [Example set conn_flg = "1", then each 項目 will be like OR A.項目1 BETWEEN :1 AND 2: ]
        /// </summary>
        public int conn_flg { get; set; } = 0;

        public ListFlexConfig() { }

        public ListFlexConfig(ListFlexConfig config)
        {
            this.flag = config.flag;
            this.cnt_start = config.cnt_start;
            this.col_alias = config.col_alias;
            this.init_csv = config.init_csv;
            this.conn_flg = config.conn_flg;
        }
    }

    /// <summary>
    /// DataGrid Item Class
    /// </summary>
    public class ListFlexItem : INotifyPropertyChanged
    {
        private int _no;
        public int No
        {
            get => _no;
            set { if (_no != value) { _no = value; OnPropertyChanged(); } }
        }

        private string _selectedItem = "";
        public string SelectedItem
        {
            get => _selectedItem;
            set { if (_selectedItem != value) { _selectedItem = value; OnPropertyChanged(); } }
        }

        private string _fromValue = "";
        public string FromValue
        {
            get => _fromValue;
            set { if (_fromValue != value) { _fromValue = value; OnPropertyChanged(); } }
        }

        private bool _isListFromEnabled = true;
        public bool IsListFromEnabled
        {
            get => _isListFromEnabled;
            set { if (_isListFromEnabled != value) { _isListFromEnabled = value; OnPropertyChanged(); } }
        }

        private string _toValue = "";
        public string ToValue
        {
            get => _toValue;
            set { if (_toValue != value) { _toValue = value; OnPropertyChanged(); } }
        }

        private bool _isListToEnabled = true;
        public bool IsListToEnabled
        {
            get => _isListToEnabled;
            set { if (_isListToEnabled != value) { _isListToEnabled = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }


    /// <summary>
    /// ListFlexView Type_Def Item
    /// </summary>
    public class TypeDefItem
    { 
        public string column_name { get; set; }
        public string data_type { get; set; }
    }
}
