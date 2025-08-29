using CvnetClient.Interface;
using CvnetClient.ViewModels;
using CvnetClient.Views;

namespace CvnetClient.Service
{
    public class DialogService : IDialogService
    {
        public Sel00ViewModel GetSel00(string mstname, string[] v_para = null, string[] v_para2 = null)
        {
            var view = new Sel00View();
            var vm = view.DataContext as Sel00ViewModel;
            if (view == null || vm == null) return null;

            // Set parameters
            vm.Mstname = mstname;
            if (v_para != null) vm.Param = v_para;
            if (v_para2 != null) vm.Param2 = v_para2;

            var ret = ClientLib.ShowDialogView(view, null);  
            if (ret != true) return null;

            return vm;
        }

        public SelShoViewModel GetSelSho()
        { 
            var view = new SelShoView();
            var vm = view.DataContext as SelShoViewModel;
            if (view == null || vm == null) return null;

            var ret = ClientLib.ShowDialogView(view, null);
            if (ret != true) return null; 
            return vm; 
        }
    }
}
