using CvnetClient.ViewModels;

namespace CvnetClient.Interface
{
    public interface IDialogService
    {
        Sel00ViewModel GetSel00(string mstname, string[] v_para = null, string[] v_para2 = null);

        SelShoViewModel GetSelSho();

        SubDlg80gphSelViewModel Get80gphSel(string[] wrk_para = null);
    }
}
