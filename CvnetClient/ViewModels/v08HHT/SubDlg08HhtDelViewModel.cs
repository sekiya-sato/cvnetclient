using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CvnetBaseCore;
using System.Data;
using CvnetClient.Models;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using CvnetClient.Utils;
using Xceed.Wpf.Toolkit.PropertyGrid.Implementation.Converters;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg08HhtDelViewModel : BaseViewModel
    {
        [ObservableProperty]
        public Dictionary<string, string>? m_selectKubun;

        [ObservableProperty]
        DateTime creditDate1;

        [ObservableProperty]
        DateTime creditDate2;

        [ObservableProperty]
        int amount;

        [ObservableProperty]
        int totalAmount;

        [ObservableProperty]
        string category;

        [ObservableProperty]
        BtListHelper? findWarehouseCd1 = new();

        [ObservableProperty]
        BtListHelper? findWarehouseCd2 = new();



        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            // Set 処理区分 ComboList
            var kubunCdList = new Dictionary<string, string>
            {
                {  "-1", "-1 全て" },
                {  "1", "1 仕入" },
                {  "2", "2 売上" },
                {  "3", "3 客数" },
                {  "4", "4 移動" },
                {  "6", "6 棚卸" },
                {  "7", "7 受注" },
                {  "0", "0 発注" },
                {  "8", "8 配分" }
            };
            SelectKubun = kubunCdList;
            Category = SelectKubun.FirstOrDefault().Key.ToString();
            CreditDate1 = DateTime.Now.AddDays(-7);
            CreditDate2 = DateTime.Now;
            FindWarehouseCd1.Code = ".";
            FindWarehouseCd2.Code = "99999999";
            Amount = 0;
            OnCheck();
            OnKensuAll();
        }

        [RelayCommand]
        public void SelectWarehouse1(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && FindWarehouseCd1 != null)
            {
                FindWarehouseCd1 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        public void SelectWarehouse2(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && FindWarehouseCd2 != null)
            {
                FindWarehouseCd2 = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }

        [RelayCommand]
        void AllDel() {
            if (Amount == 0) { ClientLib.MessageBoxOk(this, "対象件数を呼び出して下さい？"); return; }
            if (!ClientLib.MessageBox(this, "削除しますか？")) return;

            var wrk_para = new BizArray();
            wrk_para[0] = creditDate1.ToString("yyyyMMdd");
            wrk_para[1] = creditDate2.ToString("yyyyMMdd");
            wrk_para[2] = findWarehouseCd1.Code.ToString();
            wrk_para[3] = findWarehouseCd2.Code.ToString();
            if (Category!="-1") { wrk_para[4] = Category; }

            OnDelete(wrk_para);
            OnKensuAll();
            ClientLib.MessageBoxOk(this, Amount.ToString() + "件削除しました｡");
            Amount = 0;

        }

        [RelayCommand]
        void OnCheckAmountDel()
        {
            BizArray wrk_para = new BizArray();
            wrk_para[0] = FindWarehouseCd1.Code;
            wrk_para[1] = FindWarehouseCd2.Code;
            wrk_para[2] = CreditDate1.ToString();
            wrk_para[3] = CreditDate2.ToString();

            if(Category!="-1") { wrk_para[4] = Category.ToString(); }
            
            var ret_csv = OnKensu(wrk_para);
            if (ret_csv.Rows.Count == 0) return;
            Amount = Int32.Parse(ret_csv.Rows[0][0].ToString());

        }

        [RelayCommand]
        void DelUncorrect() {
            BizArray wrk_para = new BizArray();
            wrk_para[0] = FindWarehouseCd1.Code;
            wrk_para[1] = FindWarehouseCd2.Code;

            if (Category != "-1") { wrk_para[2] = Category.ToString(); }

            if (!ClientLib.MessageBox(this, "削除しますか？")) return;
            OnDelete2(wrk_para);

        }

        public DataTable OnKensu(BizArray wrk_para) 
        {
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

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str, wrk_para.ToArray());

            return ret_csv;

        }

        public void OnKensuAll()
        {
            var sql_str = " select count(*) ";
            sql_str += " from HC$Tran_HHTDATA ";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, null);
            if (ret_csv.Rows.Count == 0) return;
            TotalAmount = Int32.Parse(ret_csv.Rows[0][0].ToString());
        }

        public void OnDelete(BizArray wrk_para) 
        {
            var sql_str = " select 'D'||to_char(SEQ_NO) seq ,VDATE_UPDATE,0 col2,店舗";
            sql_str += " from HC$Tran_HHTDATA ";
            sql_str += " where 日付 between :1 and :2 ";
            sql_str += " and  店舗 between :3 and :4 ";
            if (wrk_para.Count > 4)
            {
                sql_str += " and trunc(to_number(処理区分)/10)=:5";
            }
            sql_str += " and 取込FLG<>'1'";
            sql_str += " and チェック not like 10||'%'";
            sql_str += " union all";
            sql_str += " select 'D'||to_char(SEQ_NO) seq ,VDATE_UPDATE,0 col2,店舗";
            sql_str += " from HC$Tran_HHTDATA ";
            sql_str += " where 店舗 between :3 and :4 ";
            if (wrk_para.Count > 4)
            {
                sql_str += " and trunc(to_number(処理区分)/10)=:5";
            }
            sql_str += " and 取込FLG<>'1'";
            sql_str += " and チェック like 10||'%'";

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str, wrk_para.ToArray());
            if (ret_csv.Rows.Count > 0)
            {
                var strp = "店舗";
                for (var i = 0; i < ret_csv.Rows.Count; i++)
                {
                    strp += "\n";
                    strp += ret_csv.Rows[i][0] + ",";
                    strp += ret_csv.Rows[i][1] + ",";
                    strp += ret_csv.Rows[i][2] + ",";
                    strp += ret_csv.Rows[i][3];
                }
                string[] v_para = new string[3];
                v_para[0] = new string("TRAN_HHTDATA");
                v_para[1] = new string(strp);
                v_para[2] = new string("dammy");
                var ret_csv2 = AppData.Http!.AspxSqlQuery2("many_row", v_para);
                if (!string.IsNullOrEmpty(ret_csv2))
                {
                    var lines = ret_csv2.Split('\n');
                    if (lines[0] != "0") {
                        ClientLib.MessageBoxError(this, "[ " + lines[0] + " ] 削除エラー  ");
                    }
                    else
                    {
                        ClientLib.MessageBoxOk(this, "[ " + lines[1] + " ] 正常に削除終了しました");
                    }
                }
                else
                {
                    ClientLib.MessageBoxError(this, "削除エラー");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, "範囲内のデータがありません");
            }
            return;
        }

        public void OnDelete2(BizArray wrk_para)
        {
            var sql_str = " select 'D'||to_char(SEQ_NO) seq ,VDATE_UPDATE,0 col2,店舗";
            sql_str += " from HC$Tran_HHTDATA ";
            /* sql_str += " where 日付 between :1 and :2 "; */
            sql_str += " where  店舗 between :1 and :2 ";
            if (wrk_para.Count > 2)
            {
                sql_str += " and trunc(to_number(処理区分)/10)=:3";
            }
            sql_str += " and 取込FLG='.' ";
            sql_str += " and チェック='.' ";

            var ret_csv = AppData.Http!.AspxSqlQuery(sql_str, wrk_para.ToArray());
            if (ret_csv.Rows.Count > 0)
            {
                var strp = "店舗";
                for (var i = 0; i < ret_csv.Rows.Count; i++)
                {
                    strp += "\n";
                    strp += ret_csv.Rows[i][0] + ",";
                    strp += ret_csv.Rows[i][1] + ",";
                    strp += ret_csv.Rows[i][2] + ",";
                    strp += ret_csv.Rows[i][3];
                }
                string[] v_para = new string[3];
                v_para[0] = new string("TRAN_HHTDATA");
                v_para[1] = new string(strp);
                v_para[2] = new string("dammy");
                var ret_csv2 = AppData.Http!.AspxSqlQuery2("many_row", v_para);
                if (!string.IsNullOrEmpty(ret_csv2))
                {
                    var lines = ret_csv2.Split('\n');
                    if (lines[0] != "0")
                    {
                        ClientLib.MessageBoxError(this, "[ " + lines[0] + " ] 削除エラー  ");
                    }
                    else
                    {
                        ClientLib.MessageBoxOk(this, "[ " + lines[1] + " ] 正常に削除終了しました");
                    }
                }
                else
                {
                    ClientLib.MessageBoxError(this, "削除エラー");
                }
            }
            else
            {
                ClientLib.MessageBoxError(this, "範囲内のデータがありません");
            }
            return;
        }

        #region CanDeleteProcess
        public bool CanDelete => CheckAllConditions();

        public void OnCheck()
        {
            OnPropertyChanged(nameof(CanDelete));
        }
        private bool CheckAllConditions()
        {
            return CreditDate1 <= CreditDate2 &&
                   CompareCode(FindWarehouseCd1, FindWarehouseCd2);
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
        #endregion
    }
}
