using CvnetClient.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels.Component
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
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class ListFlexItem : INotifyPropertyChanged
    {
        private int _no;
        private string _selectedItem = "";
        private string _fromValue = "";
        private string _toValue = "";

        public int No
        {
            get => _no;
            set { _no = value; OnPropertyChanged(); }
        }

        public string SelectedItem
        {
            get => _selectedItem;
            set { _selectedItem = value; OnPropertyChanged(); }
        }

        public string FromValue
        {
            get => _fromValue;
            set { _fromValue = value; OnPropertyChanged(); }
        }

        public string ToValue
        {
            get => _toValue;
            set { _toValue = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
