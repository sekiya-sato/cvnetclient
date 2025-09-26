using CvnetClient.Class;
using System.ComponentModel;

namespace CvnetClient.ViewModels
{
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
