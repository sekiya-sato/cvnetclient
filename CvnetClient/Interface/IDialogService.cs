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

        SubDlgSelShoViewModel GetSelSho(string v_mst = "", string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null, int v_kt = 0, SelValueModel sel_value = null);

        SubDlgSel2ViewModel GetSel2(string[] init_para = null, string[] init_para2 = null, string qs = "", string[] init_para3 = null);

        SubDlgSelUsrViewModel GetSelUsr(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null);

        SubDlgSelTokViewModel GetSelTok(string v_mst = "", string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null);

        SubDlgSelSirViewModel GetSelSir(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null);

        SubDlgSelKokyakuViewModel GetSelKokyaku(string v_mstname, string[] init_para = null);

        SubDlgSel10ViewModel GetSel10(string qs, string para2 = "", string[] para3 = null, string ken_cd = "", string ken_jyun = "");

        SubDlgSelPointViewModel GetSelPoint(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null);

        SubDlgSel10pViewModel GetSel10p(string qs = "", string para2 = null, string[] para3 = null, string ken_cd = "", string ken_jyun = "");

        SubDlgSel002ViewModel GetSel002(string v_mstname = "", string sql_query = "", object v_para2 = null);

        SubDlgSelclsz0ViewModel GetSelclsz0(string[] init_para = null, string[] wrk_para = null);

        SubDlg01Jan00NewViewModel GetJan00New(string[] init_para = null);

        SubDlgBcd01ViewModel GetBcd01(string[] init_para);

        SubDlgSKU01ViewModel GetSku01(string[] init_para, string[] init_para2 = null);

        SubDlgSel13ViewModel GetSel13(string[] init_para);
    }
}
