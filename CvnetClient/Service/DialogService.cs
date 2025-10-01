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

        public SubDlgSelShoViewModel GetSelSho(string v_mst = "", string[] init_para = null, string[] wrk_para = null, List<CsvItem> def = null, string[] wrk_para2 = null, int v_kt = 0)
        {
            var view = new SubDlgSelShoView();
            var vm = view.DataContext as SubDlgSelShoViewModel;
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
    }
}
