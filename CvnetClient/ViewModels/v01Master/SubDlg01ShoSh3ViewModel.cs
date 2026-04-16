using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShoSh3ViewModel : BaseViewModel
    {
        #region Binding Variables
        [ObservableProperty]
        string? m_DgTitle = "商品マスタ : 原価枝番";

        /// <summary>
        /// 商品CD (Text1)
        /// </summary>
        [ObservableProperty]
        string? m_ProdCd;

        /// <summary>
        /// 元上代 (Text2)
        /// </summary>
        [ObservableProperty]
        string? m_OriCost;

        /// <summary>
        /// 上代 (Text3)
        /// </summary>
        [ObservableProperty]
        string? m_Cost;

        /// <summary>
        /// 最終修正者 (Text7)
        /// </summary>
        [ObservableProperty]
        string? m_LastModifier;

        /// <summary>
        /// 原価 (Text4)
        /// </summary>
        [ObservableProperty]
        string? m_Price1;

        /// <summary>
        /// 営業原価 (Text5)
        /// </summary>
        [ObservableProperty]
        string? m_Price2;

        /// <summary>
        /// 加工工賃 (Text6)
        /// </summary>
        [ObservableProperty]
        string? m_Price3;

        /// <summary>
        /// 確定 Active
        /// </summary>
        [ObservableProperty]
        bool? m_btDesideAct = true;

        /// <summary>
        /// ShoSh3List DataGrid Active
        /// </summary>
        [ObservableProperty]
        bool? m_DataGridAct = true;

        [ObservableProperty]
        ObservableCollection<ShoSh3Item>? m_ShoSh3List;
        #endregion

        #region Class Variables
        private BizCsvDocument csv00; 
        public int resp_code;
        #endregion

        public int OnInit(string[] init_para, int? flg = null)
        {
            OnInitBase(init_para, flg.ToString());
            //Set 100 Row By Default
            ShoSh3List = new ObservableCollection<ShoSh3Item>(Enumerable.Range(0, 100).Select(i => new ShoSh3Item()));
            resp_code = 0;
            /* 初期化処理 */ 
            if (init_para.Length < 8) { 
                resp_code = -99; 
                ClientLib.ExitDialogResult(this, true); 
                return resp_code; 
            }
            ProdCd  = init_para[0];
            OriCost = init_para[1];
            Cost    = init_para[2];
            Price1  = init_para[3];
            Price2  = init_para[4];
            Price3  = init_para[5];
            /* 商品CD,元上代,上代,原価,営業原価,加工工賃 */
            long p_no = long.TryParse(init_para[6], out long _p_no) ? _p_no : 0;
            var ret_aspx = AppData.Http!.AspxSqlExe(DBDef.DB_DML.LOCK, "Master_SHOHIN", p_no, init_para[7], new string[0], new string[0]);
            if (ret_aspx.Code < 0)
            {
                resp_code = ret_aspx.Code;
                ClientLib.ExitDialogResult(this, true);
                return resp_code;
            }
            if (!string.IsNullOrEmpty(init_para[0]))
            {
                /****** 20070111 最終修正者追加対応 START ******/
                string sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
                sql_query += ",A.区分,A.商品CD,A.行NO,A.原価,A.営業原価,A.加工工賃";
                sql_query += ",A.入力社員CD ||' '||(select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD) 最終修正者";
                sql_query += " from HC$MASTER_SHOHIN_GENKA A";
                sql_query += " where A.商品CD=:1 and A.区分=0 and A.行NO between 1 and 100 order by A.区分,A.商品CD,A.行NO";
                /****** 20070111 最終修正者追加対応 END    ******/
                var v_para = new BizArray();
                v_para[0] = init_para[0];
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray()); 
                for (int i = 0; i < ret_csv?.Rows.Count; i++)
                {
                    int LineNo = int.TryParse(ret_csv.Rows[i][5].ToString(), out int _lineNo) ? _lineNo - 1 : 0;
                    ShoSh3List[LineNo].Cost = int.TryParse(ret_csv.Rows[i][6].ToString(), out int _cost) ? _cost : 0;
                    ShoSh3List[LineNo].OperateCost = int.TryParse(ret_csv.Rows[i][7].ToString(), out int _op_cost) ? _op_cost : 0;
                    ShoSh3List[LineNo].ProcessFee = int.TryParse(ret_csv.Rows[i][8].ToString(), out int _process_fee) ? _process_fee : 0;
                }
                /****** 20070111 最終修正者追加対応 START ******/
                if (ret_csv?.Rows.Count != 0) {
                    LastModifier = ret_csv?.Rows[0][9].ToString();
                }
                /****** 20070111 最終修正者追加対応 END    ******/
            }
            /****** 20070111 最終修正者追加対応 START ******/
            csv00 = new BizCsvDocument();
            csv00.SetColHeader("区分,商品CD,行NO,原価,営業原価,加工工賃,入力社員CD");
            /****** 20070111 最終修正者追加対応 END    ******/

            /* 参照用 */
            if (flg != null)
            {
                if (flg == 3)
                {
                    BtDesideAct = false;
                    DataGridAct = false;
                    DgTitle += "（参照用）";
                }
            }
            return resp_code;
        }

        #region Button Events
        /// <summary>
        /// 最終行追加(F11)
        /// </summary>
        [RelayCommand]
        private void DoNewRow()
        {
            if (ShoSh3List == null) return;
            ShoSh3Item item = new ShoSh3Item();
            ShoSh3List.Add(item);
        }

        /// <summary>
        /// 確定(F6)
        /// </summary>
        [RelayCommand]
        private void DoConfirm()
        { 
            int total_cnt = 0;
            if (csv00 == null) csv00 = new BizCsvDocument();
            else csv00.Clear();
            string str_csv = "区分,商品CD,行NO,原価,営業原価,加工工賃,入力社員CD \n";
            for (int i = 0; i < ShoSh3List?.Count; i++)
            {
                if (ShoSh3List[i].Cost != 0 || ShoSh3List[i].OperateCost != 0 || ShoSh3List[i].ProcessFee != 0)
                {
                    /****** 20070111 最終修正者追加対応 START ******/
                    str_csv += string.Format("{0},{1},{2},{3},{4},{5},{6} \n",
                                             "0", ProdCd, (i + 1), ShoSh3List[i].Cost, ShoSh3List[i].OperateCost, ShoSh3List[i].ProcessFee, AppData.ClassSatoo.SHAIN_CD);
                    total_cnt++;
                    /****** 20070111 最終修正者追加対応 END    ******/
                }
            }
            /* 行NO=0を登録。 */
            str_csv += string.Format("{0},{1},{2},{3},{4},{5},{6} \n",
                                      "0", ProdCd ,"0", Price1, Price2, Price3,"");
            csv00 = new BizCsvDocument(str_csv, 1);
            var v_para = new BizArray();
            v_para[0] = "MASTER_SHOHIN_GENKA";
            v_para[1] = csv00.SaveStr();
            v_para[2] = "区分=0 and 商品CD='" + ProdCd + "'"; /* 削除条件 */
            var ret_csv = AppData.Http!.AspxSqlQuery2("mi", v_para.ToArray());
            if (!string.IsNullOrEmpty(ret_csv))
            {
                string[] line_csv2 = ret_csv.Split('\n');
                if (line_csv2[0] != "0")
                    MessageBox.Show("更新エラー", "エラー", MessageBoxButton.OK);
            }
            ClientLib.ExitDialogResult(this, true);
        }

        /// <summary>
        /// 戻る(ESC)
        /// </summary>
        [RelayCommand]
        private void DoExit()
        {
            long p_no = long.TryParse(para[6], out long _p_no) ? _p_no : 0;
            var ret_aspx = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UNLOCK, "Master_SHOHIN", p_no, para[7], new string[0], new string[0]);
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion
    }

    public partial class ShoSh3Item : ObservableObject
    {
        /// <summary>
        /// 原価 (Line3)
        /// </summary>
        [ObservableProperty]
        int? m_Cost;

        /// <summary>
        /// 営業原価 (Line4)
        /// </summary>
        [ObservableProperty]
        int? m_OperateCost;

        /// <summary>
        /// 加工工賃 
        /// </summary>
        [ObservableProperty]
        int? m_ProcessFee;

        public ShoSh3Item()
        {
            Cost = 0;
            OperateCost = 0;
            ProcessFee = 0;
        }
    }
}
