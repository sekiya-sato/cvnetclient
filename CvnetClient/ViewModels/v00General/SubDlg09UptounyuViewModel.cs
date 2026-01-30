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
    public partial class SubDlg09UptounyuViewModel : BaseViewModel
    {
        [ObservableProperty]
        DateTime dateStart = DateTime.Now;

        [ObservableProperty]
        BtListHelper? findWarehouse1 = new();

        [ObservableProperty]
        BtListHelper? findWarehouse2 = new();

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);

            para = new BizArray();
            findWarehouse1.Code = ".";
            findWarehouse2.Code = "99999999";
        }

        [RelayCommand]
        void DoRun()
        {
            var wrk_para = new BizArray();
            //int shimebi = AppData.ClassCvnet.SysMst._data.Rows[0][14].ToString();
            //dateStart = AppData.ClassSatoo.GetDateVal3(dateStart, shimebi, 0);
            wrk_para[0] = new string("START");
            var ret_csv = AppData.Http?.AspxSqlQuery2("tran_tounyu", wrk_para.ToArray(),null,01);
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
        public void SelWareHouse1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                FindWarehouse1.Code = get_sel00.Code;
                FindWarehouse1.Name = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelWareHouse2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null)
            {
                FindWarehouse2.Code = get_sel00.Code;
                FindWarehouse2.Name = get_sel00.Name;
            }
        }

    }
}
