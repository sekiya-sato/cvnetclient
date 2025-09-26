using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.ViewModels;

namespace CvnetClient.Interface
{
    public interface IDialogService
    {
        SubDlgSel00ViewModel GetSel00(string mstname, string[] v_para = null, string[] v_para2 = null);

        void ShowSel00(string mstname, Action<SelValueModel> onSelected, string[] v_para = null, string[] v_para2 = null);

        SubDlg80gphSelViewModel Get80gphSel(string[] wrk_para = null);

        SubDlgSelShoViewModel GetSelSho(string v_mst = "", string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null, int v_kt = 0);

        SubDlgSel2ViewModel GetSel2(string[] init_para = null, string[] init_para2 = null, string qs = "", string[] init_para3 = null);
    }

}
