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
            DateTime Start = DateTime.Now;
            var wrk_para = new BizArray();
            int shimebi = int.Parse(AppData.ClassCvnet.SysMst._data.Rows[0][14].ToString());
            DateStart = AppData.ClassSatoo.GetDateVal3(DateStart, shimebi, 0);
            wrk_para[0] = DateStart.ToString("yyyyMMdd");
            wrk_para[1] = FindWarehouse1.Code;
            wrk_para[2] = FindWarehouse2.Code;
            var ret_csv = AppData.Http?.AspxSqlQuery2("tran_tounyu", wrk_para.ToArray(),null,01);
            var ret1 = ret_csv.Split('\n');
            if (ret1.Length < 2)
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            if (ret1[0] == "0")
            {
                var wrk_mess = "時刻：" + Start.ToString("HH24:MI:SS") + "-" + DateTime.Now.ToString("HH24:MI:SS");
                wrk_mess += "\n経過時間：" + AppData.ClassSatoo.GetDateDiff(Start, DateTime.Now);

                ClientLib.MessageBoxOk(this, "更新終了しました。\n" + wrk_mess);
            }
            else
            {
                ClientLib.MessageBoxError(this, "エラーが発生しました");
                return;
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
