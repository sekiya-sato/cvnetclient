using CvnetClient.Class;
using CvnetClient.Interface;
using CvnetClient.Models;
using CvnetClient.ViewModels;
using CvnetClient.Views;

namespace CvnetClient.Service
{
    public class DialogService : IDialogService
    {
        public SubDlgSel00ViewModel GetSel00(string mstname, string[] v_para = null, string[] v_para2 = null)
        {
            var view = new SubDlgSel00View();
            var vm = view.DataContext as SubDlgSel00ViewModel;
            if (view == null || vm == null) return null;

            // Set parameters
            vm.Mstname = mstname;
            if (v_para != null) vm.Param1 = v_para;
            if (v_para2 != null) vm.Param2 = v_para2;

            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;

            return vm;
        }

        public void ShowSel00(string mstname, Action<SelValueModel> onSelected, string[] v_para = null, string[] v_para2 = null)
        {
            var view = new SubDlgSel00View();
            var vm = view.DataContext as SubDlgSel00ViewModel;
            if (view == null || vm == null) return;

            // Set parameters
            vm.Mstname = mstname;
            if (v_para != null) vm.Param1 = v_para;
            if (v_para2 != null) vm.Param2 = v_para2;

            // Subscribe callback
            void handler(SelValueModel model)
            {
                onSelected?.Invoke(model);
                vm.SelectedItemConfirmed -= handler; // <-- unsubscribe lepas guna
                view.Close(); // kalau nak auto-close lepas pilih
            }

            vm.SelectedItemConfirmed += handler;

            ClientLib.ShowWindowView(view, null); // non-modal
        }

        public SubDlg80gphSelViewModel Get80gphSel(string[] wrk_para = null)
        {
            var view = new SubDlg80gphSelView();
            var vm = view.DataContext as SubDlg80gphSelViewModel;
            if (view == null || vm == null) return null;

            // Set parameters
            if (wrk_para != null)
                vm.Wrk_para = new Utils.BizArray(wrk_para);

            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelShoViewModel GetSelSho(string v_mst = "",
                                               string[] init_para = null,
                                               string[] wrk_para = null,
                                               List<CsvItem> def = null,
                                               string[] wrk_para2 = null,
                                               int v_kt = 0,
                                               SelValueModel sel_value = null)
        {
            var view = new SubDlgSelShoView();
            var vm = view.DataContext as SubDlgSelShoViewModel;
            vm.SelectedValue = sel_value ?? new SelValueModel();
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mst, init_para, wrk_para, def, wrk_para2, v_kt);

            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSel2ViewModel GetSel2(string[] init_para = null, string[] init_para2 = null, string qs = "", string[] init_para3 = null)
        {
            var view = new SubDlgSel2View();
            var vm = view.DataContext as SubDlgSel2ViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(init_para, init_para2, qs, init_para3);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelUsrViewModel GetSelUsr(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null)
        {
            var view = new SubDlgSelUsrView();
            var vm = view.DataContext as SubDlgSelUsrViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mst, init_para, wrk_para, def, wrk_para2);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelTokViewModel GetSelTok(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null)
        {
            var view = new SubDlgSelTokView();
            var vm = view.DataContext as SubDlgSelTokViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mst, init_para, wrk_para, def, wrk_para2);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelSirViewModel GetSelSir(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null)
        {
            var view = new SubDlgSelSirView();
            var vm = view.DataContext as SubDlgSelSirViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mst, init_para, wrk_para, def, wrk_para2);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelKokyakuViewModel GetSelKokyaku(string v_mstname, string[] init_para = null)
        {
            var view = new SubDlgSelKokyakuView();
            var vm = view.DataContext as SubDlgSelKokyakuViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mstname, init_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSel10ViewModel GetSel10(string qs, string para2 = "", string[] para3 = null, string ken_cd = "", string ken_jyun = "")
        {
            var view = new SubDlgSel10View();
            var vm = view.DataContext as SubDlgSel10ViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(qs, para2, para3, ken_cd, ken_jyun);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelPointViewModel GetSelPoint(string v_mst, string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null)
        {
            var view = new SubDlgSelPointView();
            var vm = view.DataContext as SubDlgSelPointViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mst, init_para, wrk_para, def, wrk_para2);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSel10pViewModel GetSel10p(string qs = "", string para2 = null, string[] para3 = null, string ken_cd = "", string ken_jyun = "")
        {
            var view = new SubDlgSel10pView();
            var vm = view.DataContext as SubDlgSel10pViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(qs, para2, para3, ken_cd, ken_jyun);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSel002ViewModel GetSel002(string v_mstname = "", string sql_query = "", object v_para2 = null)
        {
            var view = new SubDlgSel002View();
            var vm = view.DataContext as SubDlgSel002ViewModel;
            if (view == null || vm == null) return null;

            // Set parameters & init dialog
            vm.OnInit(v_mstname, sql_query, v_para2);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelclsz0ViewModel GetSelclsz0(string[] init_para = null, string[] wrk_para = null)
        {
            var view = new SubDlgSelclsz0View();
            var vm = view.DataContext as SubDlgSelclsz0ViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para, wrk_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlg01Jan00NewViewModel GetJan00New(string[] init_para = null)
        {
            var view = new SubDlg01Jan00NewView();
            var vm = view.DataContext as SubDlg01Jan00NewViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgBcd01ViewModel GetBcd01(string[] init_para)
        {
            var view = new SubDlgBcd01View();
            var vm = view.DataContext as SubDlgBcd01ViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSKU01ViewModel GetSku01(string[] init_para, string[] init_para2 = null)
        {
            var view = new SubDlgSKU01View();
            var vm = view.DataContext as SubDlgSKU01ViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para, init_para2);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSel13ViewModel GetSel13(string[] init_para)
        {
            var view = new SubDlgSel13View();
            var vm = view.DataContext as SubDlgSel13ViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlg01ShoSh2ViewModel GetShoSh2(string[] init_para, int flg)
        {
            var view = new SubDlg01ShoSh2View();
            var vm = view.DataContext as SubDlg01ShoSh2ViewModel;

            // Set parameters & init dialog 
            var resp_code = vm.OnInit(init_para, flg);
            if (resp_code != 0) return vm;

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelViewModel GetSel(string[] init_para)
        {
            var view = new SubDlgSelView();
            var vm = view.DataContext as SubDlgSelViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlgSelghnViewModel GetSelghn(string[] init_para)
        {
            var view = new SubDlgSelghnView();
            var vm = view.DataContext as SubDlgSelghnViewModel;

            // Set parameters & init dialog
            vm.OnInit(init_para);

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlg01ShoSh3ViewModel GetShoSh3(string[] init_para, int? flg = null)
        {
            var view = new SubDlg01ShoSh3View();
            var vm = view.DataContext as SubDlg01ShoSh3ViewModel;

            // Set parameters & init dialog
            var resp_code = vm.OnInit(init_para, flg); 
            if (resp_code != 0) return vm;

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }

        public SubDlg01ShoSh5v2ViewModel GetShoSh5v2(string[] init_para, int? flg = null)
        {
            var view = new SubDlg01ShoSh5v2View();
            var vm = view.DataContext as SubDlg01ShoSh5v2ViewModel;

            // Set parameters & init dialog
            var resp_code = vm.OnInit(init_para, flg);
            if (resp_code != 0) return vm;

            // Show dialog and wait return value
            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
        }
    }
}
