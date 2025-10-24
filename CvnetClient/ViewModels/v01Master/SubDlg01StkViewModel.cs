using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using System;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01StkViewModel : BaseViewModel
    {
        private BizArray para;
        private BizArray sv_cat;
        private BizArray wrk_jodai;

        [ObservableProperty] private ListFlexData listFlexData = new();
        [ObservableProperty] private StkSearchOpt searchOpt = new();

        // -------------------- 得意先／仕入先 --------------------
        [ObservableProperty] private int selCustomerSupplier = 0; // 0=得意先, 1=仕入先
        [ObservableProperty] private string labelRange = "得意先 範囲";

        // -------------------- 範囲 (FROM / TO) --------------------
        [ObservableProperty] private MasterCodeName? rangeFrom = new();
        [ObservableProperty] private MasterCodeName? rangeTo = new();

        // ----------------------------------------------------------
        // 初期化
        // ----------------------------------------------------------
        public void OnInit(string[] init_para = null)
        {
            para = new BizArray();
            sv_cat = new BizArray();
            wrk_jodai = new BizArray();

            if (init_para != null)
                para = new BizArray(init_para);

            SearchOpt = new StkSearchOpt();
        }

        partial void OnSelCustomerSupplierChanged(int value)
        {
            LabelRange = (value == 0 ? "得意先" : "仕入先") + " 範囲";
        }

        // ----------------------------------------------------------
        // 🔍 Range-Select Buttons
        // ----------------------------------------------------------
        [RelayCommand]
        public void SelRangeFrom(object value)
        {
            if (value is SelValueModel get_sel00)
            {
                RangeFrom ??= new();
                RangeFrom.Code = get_sel00.Code;
                RangeFrom.Name = get_sel00.Name;
            }
        }

        [RelayCommand]
        public void SelRangeTo(object value)
        {
            if (value is SelValueModel get_sel00)
            {
                RangeTo ??= new();
                RangeTo.Code = get_sel00.Code;
                RangeTo.Name = get_sel00.Name;
            }
        }

        // ----------------------------------------------------------
        // 🖨️ Execute (印刷)
        // ----------------------------------------------------------
        [RelayCommand]
        private void DoExecute()
        {
            var wrk_para = new BizArray();
            wrk_para[0] = RangeFrom?.Code ?? "000001";
            wrk_para[1] = RangeTo?.Code ?? "999999";

            var wrk_para2 = new BizArray();
            wrk_para2[0] = "印刷日付";
            wrk_para2[1] = "担当者名";

            string v_sql = BuildJoinCondition();

            string query = OnQueryList(wrk_para, wrk_para2, v_sql);
            System.Diagnostics.Debug.WriteLine($"Query executed for {(SelCustomerSupplier == 0 ? "得意先" : "仕入先")}");
            System.Diagnostics.Debug.WriteLine(query);

            // Later: bind result or send to print
            // var result = AppData.Http.AspxSqlQuery(query, wrk_para.ToArray(), "cvnet_takku.qfm");
        }

        // ----------------------------------------------------------
        // 戻る
        // ----------------------------------------------------------
        [RelayCommand]
        private void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }

        // ----------------------------------------------------------
        // AND／OR 結合条件 SQL 部分
        // ----------------------------------------------------------
        private string BuildJoinCondition()
        {
            string cond = SearchOpt.SelJoinCond == 0 ? " AND " : " OR ";
            return $"{cond} 削除区分='0'";
        }

        // ----------------------------------------------------------
        // CRS OnQueryList() 相当
        // ----------------------------------------------------------
        private string OnQueryList(BizArray wrk_para, BizArray wrk_para2, string v_sql)
        {
            int now_sel = SelCustomerSupplier;
            string sql_query;
            string houjin_str = AppData.ClassCvnet.GetQueryStrHoujin();

            if (now_sel == 0)
            {
                sql_query = "select 得意先CD, 得意先名, 郵便番号, 住所1, 住所2, 住所3, 宛名名称1, 宛名名称2";
                sql_query += $", '{wrk_para2.Get(0)}', '{wrk_para2.Get(1)}' ";
                sql_query += "from HC$MASTER_TOKUI ";
                sql_query += "where 得意先CD between :1 and :2 ";
                sql_query += v_sql;
                sql_query += houjin_str;
                sql_query += " order by 得意先CD";
            }
            else
            {
                sql_query = "select 仕入先CD, 仕入先名, 郵便番号, 住所1, 住所2, 住所3, 宛名名称1, 宛名名称2";
                sql_query += $", '{wrk_para2.Get(0)}', '{wrk_para2.Get(1)}' ";
                sql_query += "from HC$MASTER_SIIRE ";
                sql_query += "where 仕入先CD between :1 and :2 ";
                sql_query += v_sql;
                sql_query += houjin_str;
                sql_query += " order by 仕入先CD";
            }

            System.Diagnostics.Debug.WriteLine($"sql_query={sql_query}");
            return sql_query;
        }
    }

    // ----------------------------------------------------------
    // 検索オプション
    // ----------------------------------------------------------
    public partial class StkSearchOpt : ObservableObject
    {
        [ObservableProperty] private int selJoinCond = 0;     // 0=AND, 1=OR
        [ObservableProperty] private int addressCustomer = 0; // 宛名修飾文字
    }

    // ----------------------------------------------------------
    // 範囲 (コード＋名称)
    // ----------------------------------------------------------
    public partial class MasterCodeName : ObservableObject
    {
        [ObservableProperty] private string? code;
        [ObservableProperty] private string? name;
    }
}
