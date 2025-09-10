using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input; 
using CvnetClient.Models; 
using System.Data; 

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSel00ViewModel : BaseViewModel
    {

        public event Action<Sel00Model>? SelectedItemConfirmed;

        [ObservableProperty]
        string[] param = null;

        [ObservableProperty]
        string[] param2 = null;

        bool is1stInit = true;
        List<Sel00Model>? listSel00Ori;

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
            DataTable ret_csv = AppData.ClassCvnet.AspxSqlQueryMst(mstname, Param, Param2, 1);
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
            if (is1stInit == true) { listSel00Ori = new List<Sel00Model>(ListSel00); is1stInit = false; }
        }

        [RelayCommand]
        void NextList()
        { 
            if (ListSel00 != null && ListSel00.Count == 0) return;
            Param = new string[1];
            Param[0] = ListSel00.Last().Code; 
        }

        [RelayCommand]
        void TopList()
        {
            if (listSel00Ori != null && listSel00Ori.Count == 0) return;
            ListSel00 = new List<Sel00Model>(listSel00Ori);
        }

        [RelayCommand]
        void NameSearch()
        {
            if (ListSel00 != null && ListSel00.Count == 0) return;
            if (string.IsNullOrEmpty(Textmeisho))
                ListSel00 = new List<Sel00Model>(listSel00Ori);
            else ListSel00 = ListSel00.Where(x => x.Name.Contains(Textmeisho)).ToList();
        }

        [RelayCommand]
        void DoSearch()
        {
            if(SelectSel00 == null) return;
            //confirm and close window
            SelectedItemConfirmed?.Invoke(SelectSel00);
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
