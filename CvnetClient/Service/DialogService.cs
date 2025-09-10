using CvnetClient.Interface;
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
            if (v_para != null) vm.Param = v_para;
            if (v_para2 != null) vm.Param2 = v_para2;

            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null; 
            return vm;
        }

        public SubDlgSelShoViewModel GetSelSho()
        {
            var view = new SubDlgSelShoView();
            var vm = view.DataContext as SubDlgSelShoViewModel;
            if (view == null || vm == null) return null;

            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null;
            return vm;
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
    }
}
