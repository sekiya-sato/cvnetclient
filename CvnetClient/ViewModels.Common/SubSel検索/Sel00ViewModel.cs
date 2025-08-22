using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CvnetClient.ViewModels
{
    public partial class Sel00ViewModel : BaseViewModel
    {  
        public event EventHandler<bool> RequestClose;
          
        string[] v_para = null;

        [ObservableProperty]
        string mstname = string.Empty; 

        [ObservableProperty]
        List<Sel00Model>? listSel00;

        [ObservableProperty]
        Sel00Model? selectSel00;

        [ObservableProperty]
        string _textmeisho = "";

        [RelayCommand]
        void Init()
        {
            DataTable ret_csv = cvnet.AspxSqlQueryMst(mstname, v_para, null, 1);
            ListSel00 = new List<Sel00Model>();
            foreach (DataRow row in ret_csv.Rows)
            {
                var model = new Sel00Model
                {
                    Code = row[0] != DBNull.Value ? row[0].ToString() : string.Empty,
                    Name = row[1] != DBNull.Value ? row[1].ToString() : string.Empty
                };
                ListSel00.Add(model);
            }
        }

        [RelayCommand]
        void NextList()
        { 
        }

        [RelayCommand]
        void TopList()
        { 
        
        }

        [RelayCommand]
        void NameSearch()
        {
            
        }

        [RelayCommand]
        void DoSearch()
        {
            //confirm and close window
            RequestClose?.Invoke(this, true);
        }
    }
}
