using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.ViewModels.v00General;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg00Menu01ViewModel : BaseViewModel
    {
        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            para = new BizArray();

        }

        [RelayCommand]
        public void OpenAutorun()
        {
            // 1️⃣ Create a new parameter list (BizArray in your project)
            BizArray newPara = new BizArray();

            // Add as many as needed
            string newFlg = "0"; // if needed


            // Create the child ViewModel
            var vm = new SubDlg90Jidou08ViewModel();

            // 3️⃣ Initialize the child VM with the new parameters
            vm.OnInitBase(newPara, newFlg);

            var view = new SubDlg90Jidou08View
            {
                DataContext = vm
            };

            // 5️⃣ Show NON-MODAL (or use ShowDialogView for modal)
            ClientLib.ShowWindowView(
                childWin: view,
                myVm: this,                      // Owner window
                loc: WindowStartupLocation.CenterOwner,
                isShowTaskbar: false
            );
        }

        [RelayCommand]
        public void OpenRunHandy()
        {
            // 1️⃣ Create a new parameter list (BizArray in your project)
            BizArray newPara = new BizArray();

            // Add as many as needed
            string newFlg = "0"; // if needed


            // Create the child ViewModel
            var vm = new SubDlg00HhtflgchkViewModel();

            // 3️⃣ Initialize the child VM with the new parameters
            vm.OnInitBase(newPara, newFlg);

            var view = new SubDlg00HhtflgchkView
            {
                DataContext = vm
            };

            // 5️⃣ Show NON-MODAL (or use ShowDialogView for modal)
            ClientLib.ShowWindowView(
                childWin: view,
                myVm: this,                      // Owner window
                loc: WindowStartupLocation.CenterOwner,
                isShowTaskbar: false
            );
        }

        


        [RelayCommand]
        public void OpenUpdateSupply()
        {
            // 1️⃣ Create a new parameter list (BizArray in your project)
            BizArray newPara = new BizArray();

            // Add as many as needed
            string newFlg = "0"; // if needed


            // Create the child ViewModel
            var vm = new SubDlg09UptounyuViewModel();

            // 3️⃣ Initialize the child VM with the new parameters
            vm.OnInitBase(newPara, newFlg);

            var view = new SubDlg09UptounyuView
            {
                DataContext = vm
            };

            // 5️⃣ Show NON-MODAL (or use ShowDialogView for modal)
            ClientLib.ShowWindowView(
                childWin: view,
                myVm: this,                      // Owner window
                loc: WindowStartupLocation.CenterOwner,
                isShowTaskbar: false
            );
        }
    }
}
