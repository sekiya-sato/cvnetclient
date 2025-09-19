using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 
using CvnetClient.Models;
using System.Collections.ObjectModel;
using System.Data; 

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel00ViewModel : BaseViewModel
    {

        public event Action<SelValueModel>? SelectedItemConfirmed;

        [ObservableProperty]
        string[] param1 = null;

        [ObservableProperty]
        string[] param2 = null;

        int nullflg = 0;

        bool is1stInit = true;
        List<Sel00Model>? listSel00Ori;

        [ObservableProperty]
        string mstname = string.Empty; 

        [ObservableProperty]
        ObservableCollection<Sel00Model>? listSel00;

        [ObservableProperty]
        Sel00Model? selectedSel00;

        [ObservableProperty]
        SelValueModel? selectedValue;

        [ObservableProperty]
        string _textmeisho = "";

        [RelayCommand]
        void Init()
        {
            DataTable ret_csv = AppData.ClassCvnet.AspxSqlQueryMst(Mstname, Param1, Param2, 1);
            ListSel00 = new ObservableCollection<Sel00Model>();
            foreach (DataRow row in ret_csv.Rows)
            {
                var model = new Sel00Model
                {
                    Code = row[0] != DBNull.Value ? row[0].ToString() : string.Empty,
                    Name = row[1] != DBNull.Value ? row[1].ToString() : string.Empty
                };
                ListSel00.Add(model);
            }
            if (is1stInit == true) { listSel00Ori = new List<Sel00Model>(ListSel00); is1stInit = false; }
        }

        public void OnInit(string v_mstname, string[] init_para = null, string[] v_para2 = null)
        {
            Mstname = v_mstname;
            Param1 = init_para;
            Param2 = v_para2;
            OnInit2();
        }

        void OnInit2()
        {
            if (Param1 == null) return;
            if (Param1.Length > 0)
            {
                ListSel00 = new ObservableCollection<Sel00Model>();
                DataTable ret_csv = AppData.ClassCvnet.AspxSqlQueryMst(Mstname, Param1, Param2, 1);
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
            if (is1stInit == true) { listSel00Ori = new List<Sel00Model>(ListSel00.ToList()); is1stInit = false; }
        }

        [RelayCommand]
        void NextList()
        { 
            if (ListSel00 != null && ListSel00.Count == 0) return;
            Param1 = new string[1];
            Param1[0] = ListSel00.Last().Code; 
        }

        [RelayCommand]
        void TopList()
        {
            if (listSel00Ori != null && listSel00Ori.Count == 0) return;
            ListSel00 = new ObservableCollection<Sel00Model>(listSel00Ori);
        }

        [RelayCommand]
        void NameSearch()
        {
            if (ListSel00 != null && ListSel00.Count == 0) return;
            if (string.IsNullOrEmpty(Textmeisho))
                ListSel00 = new ObservableCollection<Sel00Model>(listSel00Ori);
            else ListSel00 = new ObservableCollection<Sel00Model>(ListSel00.Where(x => x.Name.Contains(Textmeisho)).ToList());
        }

        [RelayCommand]
        void DoSearch()
        {
            if(SelectedSel00 == null) return;
            SelectedValue = new SelValueModel()
            {
                Code = SelectedSel00.Code,
                Name = SelectedSel00.Name
            }; 
            //confirm and close window
            SelectedItemConfirmed?.Invoke(SelectedValue);
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        void DoExit()
        {
            //confirm and close window
            ClientLib.ExitDialogResult(this, true);
        }
    }
}
