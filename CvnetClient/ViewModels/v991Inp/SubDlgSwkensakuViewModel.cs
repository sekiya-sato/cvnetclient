using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgSwkensakuViewModel : BaseViewModel
    {
        [ObservableProperty]
        public string? m_Title = "ｽﾜｯﾁ検索";

        [ObservableProperty]
        public int? m_SwatchPage;

        [ObservableProperty]
        public int? m_SwatchPosition;

        public BizArray ret_para;

        #region Events
        [RelayCommand]
        public void DoExecute()
        {
            if (SwatchPage == null || SwatchPosition == null) return;
            ret_para = new BizArray();
            ret_para[0] = SwatchPage.ToString();
            ret_para[1] = SwatchPosition.ToString();
            ClientLib.ExitDialogResult(this, true);
        }

        [RelayCommand]
        public void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }
}
