using CvnetClient.Models;
using CvnetClient.ViewModels;

namespace CvnetClient.Interface
{
    public interface IDialogService
    {
        SubDlgSel00ViewModel GetSel00(string mstname, string[] v_para = null, string[] v_para2 = null);

        void ShowSel00(string mstname, Action<SelValueModel> onSelected, string[] v_para = null, string[] v_para2 = null);
         
        SubDlgSelShoViewModel GetSelSho();

        SubDlg80gphSelViewModel Get80gphSel(string[] wrk_para = null);

        SubDlgSel2ViewModel GetSel2(string[] init_para = null, string[] init_para2 = null, string qs = "", string[] init_para3 = null);
    }

}
