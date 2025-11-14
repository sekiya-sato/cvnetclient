using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CvnetBaseCore;
using System.Data;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using CvnetClient.Utils;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08HhtDelViewModel : BaseViewModel
    {
        [ObservableProperty]
        public Dictionary<int, string>? m_selectKubun;

        [ObservableProperty]
        DateTime creditDate1;

        [ObservableProperty]
        DateTime creditDate2;

        [ObservableProperty]
        int amount;

        [ObservableProperty]
        int totalAmount;

        [ObservableProperty]
        int category;


        [ObservableProperty]
        BtListHelper? findWarehouseCd1 = new();

        [ObservableProperty]
        BtListHelper? findWarehouseCd2 = new();

        [RelayCommand]
        void Init(object? init_para)
        {
            // Set 処理区分 ComboList
            SelectKubun = new Dictionary<int, string>
            {
                {  -1, "-1 全て" },
                {  1, "1 仕入" },
                {  2, "2 売上" },
                {  3, "3 客数" },
                {  4, "4 移動" },
                {  6, "6 棚卸" },
                {  7, "7 受注" },
                {  0, "0 発注" },
                {  8, "8 配分" }
            };
            Category = SelectKubun.FirstOrDefault().Key;
            CreditDate1 = DateTime.Now.AddDays(-7);
            CreditDate2 = DateTime.Now;

            OnCheck();
            OnKensuAll();
        }

        [RelayCommand]
        public void SelectWarehouse1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && findWarehouseCd1 != null)
            {
                findWarehouseCd1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelectWarehouse2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && findWarehouseCd2 != null)
            {
                findWarehouseCd2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }


        [RelayCommand]
        void AllDel() {
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;

        }

        [RelayCommand]
        void OnBtnCheck()
        {
            BizArray wrk_para = new BizArray();
            wrk_para[0] = findWarehouseCd1.Code;
            wrk_para[1] = findWarehouseCd2.Code;
            wrk_para[2] = creditDate1.ToString();
            wrk_para[3] = creditDate2.ToString();

            if(category!=-1) { wrk_para[4] = category.ToString(); }
            var sql_str = "SELECT a.その他エラー件数+b.登録日エラー件数 エラー件数";
            sql_str += " FROM ";
            sql_str += "(SELECT count(*) その他エラー件数"
                        + " FROM HC$Tran_HHTDATA"
                        + " where 日付 between :1 and :2 "
                        + " and  店舗 between :3 and :4 ";
            if (wrk_para.Count > 4)
            {
                sql_str += " and trunc(to_number(処理区分)/10)=:5";
            }
            sql_str += " and 取込FLG<>'1'";
            sql_str += " and チェック not like 10||'%'";
            sql_str += " ) a ";
            sql_str += ",(SELECT count(*) 登録日エラー件数"
                        + " FROM HC$Tran_HHTDATA"
                        + " WHERE 店舗 between :3 and :4 ";
            if (wrk_para.Count > 4)
            {
                sql_str += " and trunc(to_number(処理区分)/10)=:5";
            }
            sql_str += " and 取込FLG<>'1'";
            sql_str += " and チェック like 10||'%'";
            sql_str += " ) b ";

        }

        public void OnCheck()
        {
            OnPropertyChanged(nameof(CanDelete));
        }

        public bool CanDelete => CheckAllConditions();

        private bool CheckAllConditions()
        {
            return CreditDate1 <= CreditDate2 &&
                   CompareCode(findWarehouseCd1, findWarehouseCd2);
        }

        private bool CompareCode(BtListHelper? a, BtListHelper? b)
        {
            if (a == null || b == null) return false;
            return string.Compare(a.Code, b.Code, StringComparison.Ordinal) <= 0;
        }

        partial void OnCreditDate1Changed(DateTime value) => OnCheck();
        partial void OnCreditDate2Changed(DateTime value) => OnCheck();
        partial void OnFindWarehouseCd1Changed(BtListHelper? value) => OnCheck();
        partial void OnFindWarehouseCd2Changed(BtListHelper? value) => OnCheck();

        [RelayCommand]
        void DelUncorrect() { 
        
        }

        public void OnKensuAll()
        {
            var sql_str = " select count(*) ";
            sql_str += " from HC$Tran_HHTDATA ";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, null);
            if (ret_csv.Rows.Count == 0) return;
            TotalAmount = Int32.Parse(ret_csv.Rows[0][0].ToString());
        }
    }
}
