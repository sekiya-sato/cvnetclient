using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg90Jidou08ViewModel : BaseViewModel
    {
        [ObservableProperty]
        string? status;

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            para = new BizArray();
            var wrk_para = new BizArray();
            wrk_para[0] = new string("RUNNING");
            var ret_csv = AppData.Http?.AspxSqlQuery2("KYOUWA_TIMER", wrk_para.ToArray());
            var ret1 = ret_csv.Split('\n');
            if (ret1.Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            if (ret1[0] == "1")
            {
                Status = "進行中";
                //Form1.Text1.BgColor =$STD;
            }
            else
            {
                Status = "停止";
                //Form1.Text1.BgColor =$RED;
            }
        }

        [RelayCommand]
        void DoRun()
        {
            var wrk_para = new BizArray();
            wrk_para[0] = new string("START");
            var ret_csv = AppData.Http?.AspxSqlQuery2("KYOUWA_TIMER", wrk_para.ToArray());
            var ret1 = ret_csv.Split('\n');
            if (ret1.Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            if (ret1[0] == "0")
            {
                Status = "進行中";
                //Form1.Text1.BgColor =$STD;
            }
            else
            {
                ClientLib.MessageBoxError(this, ret1[0]);
            }
                
        }

        [RelayCommand]
        void DoStop()
        {
            var wrk_para = new BizArray();
            wrk_para[0] = new string("END");
            var ret_csv = AppData.Http?.AspxSqlQuery2("KYOUWA_TIMER", wrk_para.ToArray());
            var ret1 = ret_csv.Split('\n');
            if (ret1.Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            if (ret1[0] == "0")
            {
                Status = "停止";
                //Form1.Text1.BgColor =$STD;
            }
            else
            {
                ClientLib.MessageBoxError(this, ret1[0]);
            }

        }

    }
}
