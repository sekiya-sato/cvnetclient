using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShoSh2ViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        string m_DlgTitle = "商品マスタ : 品質表示";

        /// <summary>
        /// 商品CD (Text3)
        /// </summary>
        [ObservableProperty]
        string m_ProdCD = "";

        /// <summary>
        /// 商品名 (Text4)
        /// </summary>
        [ObservableProperty]
        string m_ProdName = "";

        /// <summary>
        /// 最終修正者 (Text1)
        /// </summary>
        [ObservableProperty]
        string m_LastModifier = "";

        /// <summary>
        /// 作成日 (Text2)
        /// </summary>
        [ObservableProperty]
        string m_VdateCreate = "";

        /// <summary>
        /// 最終行追加(F11) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_BtAddVis = Visibility.Visible;

        /// <summary>
        /// 確定(F6) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_BtDesideVis = Visibility.Visible;

        [ObservableProperty]
        Dictionary<string, string>? m_HnkList;

        [ObservableProperty]
        ObservableCollection<ShoSh2FlexItem>? m_FlexList;
        #endregion

        #region Class Variable
        public int resp_code;
        private BizCsvDocument hin_csv;
        private int Dlg_Flg = 0;
        #endregion

        public int OnInit(string[] init_para, int? flg = null)
        {
            resp_code = 0;
            /* 初期化処理 */
            if (init_para.Length < 4) { 
                resp_code = -99;
                ClientLib.ExitDialogResult(this, true);
                return resp_code; 
            }

            HNKSet(); /* 品質表示区分セット */
            if (flg == null)
            {
                long para2 = long.TryParse(init_para[2], out long _para2) ? _para2 : 0;
                var ret_aspx = AppData.Http!.AspxSqlExe(CvnetBaseCore.DBDef.DB_DML.LOCK,
                                                        "Master_SHOHIN",
                                                        para2, init_para[3],
                                                        null, null);
                if (ret_aspx.Code < 0) { 
                    resp_code = ret_aspx.Code;
                    ClientLib.ExitDialogResult(this, true);
                    return resp_code; 
                }
            }

            if (!string.IsNullOrEmpty(init_para[0]))
            {
                ProdCD   = init_para[0];
                ProdName = init_para[1];

                /****** 20070111 最終修正者追加対応 START ******/
                string sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
                sql_query += ",A.品質,A.\"パーセント\"";
                sql_query += ",A.入力社員CD ||' '||(select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD) 最終修正者";
                sql_query += ",A.生地付属CD ,A.生地付属CD ||' '||(select M.名称 from HC$MASTER_MEISHO M where M.名称区分='HNK' AND M.名称CD=A.生地付属CD) 生地区分";
                sql_query += " from HC$Master_SHOHIN_GRADE A";
                sql_query += " where A.商品CD=:1";
                sql_query += " order by A.行NO,A.SEQ_NO";
                /****** 20070111 最終修正者追加対応 END    ******/

                var v_para = new BizArray();
                v_para[0] = init_para[0];
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
                var list = (from DataRow dr in ret_csv.Rows
                            select new ShoSh2FlexItem
                            {
                                SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                                VdateCreate = Convert.ToDouble(dr["VDATE_CREATE"]),
                                VdateUpdate = Convert.ToDouble(dr["VDATE_UPDATE"]),
                                QualityName = dr["品質"].ToString() ?? string.Empty,
                                Percent = Convert.ToInt32(dr["パーセント"]),
                                LastModifier = dr["最終修正者"].ToString() ?? string.Empty,
                                FabricCert = dr["生地付属CD"].ToString() ?? string.Empty
                            }).OrderBy(c => c.SeqNo).ToList();
                FlexList = new ObservableCollection<ShoSh2FlexItem>(list);

                /****** 20070111 最終修正者追加対応 START ******/
                if (FlexList.Count > 0)
                {
                    LastModifier = FlexList.FirstOrDefault().LastModifier;
                    VdateCreate = AppData.ClassSatoo.GetVdate(FlexList.FirstOrDefault().VdateCreate ?? 0).ToString();
                }
                /****** 20070111 最終修正者追加対応 END    ******/
            }

            /* 参照用 */
            if (flg != null)
            {
                if (flg == 3) {
                    BtAddVis = Visibility.Collapsed;
                    BtDesideVis = Visibility.Collapsed;
                    DlgTitle += "（参照用）";
                    flg = 1;
                }
            } 
            return resp_code;
        }

        #region Button Events
        /// <summary>
        /// 品質一覧
        /// </summary>
        /// <param name="item"></param>
        [RelayCommand]
        private void DoQualitySearch(ShoSh2FlexItem item)
        {
            var wrk_para = new BizArray();
            wrk_para[0] = "HIN";
            wrk_para[1] = "";
            if (hin_csv == null) hin_csv = new BizCsvDocument();
            if (hin_csv.csv_table.Rows.Count <= 0)
            {
                string sql_query = "select 名称CD,名称 from HC$MASTER_MEISHO ";
                sql_query += " where 名称区分='HIN' order by 名称CD";
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query);
                if (ret_csv != null)
                    hin_csv = new BizCsvDocument(ret_csv);
                wrk_para[2] = hin_csv.SaveStr(0);
            }
            else {
                wrk_para[2] = hin_csv.SaveStr(0);
            }
            Dlg_Flg = 11;
            var result = AppData.DlgService.GetSel(wrk_para.ToArray());
            if (result != null && result.ret_result != null)
            {
                if (Dlg_Flg == 11)
                {
                    item.QualityName = result.ret_result[1];
                }
            } 
        }

        /// <summary>
        /// 最終行追加(F11)
        /// </summary>
        [RelayCommand]
        private void DoNewRow() {
            if (FlexList == null) return;
            ShoSh2FlexItem item = new ShoSh2FlexItem();
            item.FabricCert = HnkList.FirstOrDefault().Key;
            FlexList.Add(item);
        }

        /// <summary>
        /// 確定(F6)
        /// </summary>
        [RelayCommand]
        private void DoConfirm() {
            bool hasQntNameEmpty = FlexList?.Any(x => string.IsNullOrEmpty(x.QualityName)) ?? false;
            if (hasQntNameEmpty)
            {
                MessageBox.Show("空白行があります！", "エラー", MessageBoxButton.OK);
                return;
            }
            var col_list = new BizArray();
            col_list[0] = "TB_NAME";
            col_list[1] = "TB_COL03";
            col_list[2] = "TB_COL04";
            col_list[3] = "TB_COL05";
            col_list[4] = "MOD_SEQ";
            /****** 20070111 最終修正者追加対応 START ******/
            col_list[5] = "TB_COL06";
            /****** 20070111 最終修正者追加対応 END    ******/
            /* 行No追加 */
            col_list[6] = "TB_COL07";
            /* 生地付属・生地付属行NO追加 */
            col_list[7] = "TB_COL08";
            col_list[8] = "TB_COL09";

            var paralist = new BizArray();
            paralist[0] = "Master_SHOHIN_GRADE";
            paralist[1] = ProdCD;
            long ret_min = -1;
            long ret_max = 0;
            int ki = 0;
            string old_kiji = "";
            /* 品質/パーセント/区分をセット */  
            for (int i = 0; i < FlexList?.Count; i++)
            {
                paralist[2] = FlexList[i].QualityName;
                paralist[3] = FlexList[i].Percent.ToString();
                paralist[4] = (i + 1).ToString();
                /****** 20070111 最終修正者追加対応 START ******/
                paralist[5] = AppData.ClassSatoo.SHAIN_CD;
                /****** 20070111 最終修正者追加対応 END    ******/
                /* 行No追加 */
                paralist[6] = (i + 1).ToString();
                /* 生地付属・生地付属行NO追加 ダミー的に */
                if (FlexList[i].FabricCert != old_kiji)
                {
                    ki = 1;
                }
                paralist[7] = FlexList[i].FabricCert; 	/* 生地付属に区分をSET */
                paralist[8] = ki.ToString(); /* 区分ごとの行NOをセット */
                var ret_aspx = AppData.Http!.AspxSqlExe(CvnetBaseCore.DBDef.DB_DML.INSERT, "Work_WRK001", 0, "0", col_list.ToArray(), paralist.ToArray());
                if (ret_min < 0) ret_min = ret_aspx.NewSeq;
                else
                { 
                    if (ret_min > ret_aspx.NewSeq) ret_min = ret_aspx.NewSeq;
                }
                if (ret_max < ret_aspx.NewSeq) ret_max = ret_aspx.NewSeq;
                if (ret_aspx.Code != 0)
                {
                    MessageBox.Show("更新エラー", "エラー", MessageBoxButton.OK);
                    return;
                }
                old_kiji = FlexList[i].FabricCert;
                ki++; 
            }
            paralist.Clear();
            paralist[0] = "Master_SHOHIN_GRADE";
            paralist[1] = ret_min.ToString();
            paralist[2] = ret_max.ToString();
            paralist[3] = ProdCD;
            var ret_csv = AppData.Http?.AspxSqlQuery2("tran_shohin", paralist.ToArray());
            var ret1 = ret_csv.Split(',');
            if (ret1[0] != "0")
            {
                MessageBox.Show("更新エラー", "エラー", MessageBoxButton.OK);
                return;
            }
            MessageBox.Show("登録完了", "成功", MessageBoxButton.OK);
            ClientLib.ExitDialogResult(this, true);
        }

        /// <summary>
        /// 戻る
        /// </summary>
        [RelayCommand]
        private void DoExit()
        {
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion

        #region Function
        private void HNKSet()
        {
            var hnk_list = new Dictionary<string, string>();
            /* 品質表示リスト */
            string sql_query = "select 名称CD, 名称CD||' '||名称 区分名 from HC$MASTER_MEISHO "
                + " where 名称区分='HNK' order by 名称CD";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_query);
            string wrkitem = "";
            if (ret_csv != null && ret_csv.Rows.Count > 0)
            {
                for (int i = 0; i < ret_csv.Rows.Count; i++)
                {
                    hnk_list.Add(ret_csv.Rows[i][0].ToString(), ret_csv.Rows[i][1].ToString());
                } 
            }
            HnkList = hnk_list;
        }
        #endregion
    }

    public partial class ShoSh2FlexItem : ObservableObject
    {
        /// <summary>
        /// SEQ_NO
        /// </summary>
        [ObservableProperty]
        long? m_SeqNo;

        /// <summary>
        /// VDATE_CREATE
        /// </summary>
        [ObservableProperty]
        double? m_VdateCreate;

        /// <summary>
        /// VDATE_UPDATE
        /// </summary>
        [ObservableProperty]
        double? m_VdateUpdate;
         
        /// <summary>
        /// 品質名
        /// </summary>
        [ObservableProperty]
        string? m_QualityName;

        /// <summary>
        /// パーセント
        /// </summary>
        [ObservableProperty]
        int? m_Percent;

        /// <summary>
        /// 最終修正者
        /// </summary>
        [ObservableProperty]
        string? m_LastModifier;

        /// <summary>
        /// 生地区分
        /// </summary>
        [ObservableProperty]
        string? m_FabricCert;

        public ShoSh2FlexItem()
        { 
            SeqNo = 0;
            VdateCreate = 0;
            VdateUpdate = 0;
            QualityName = "";
            Percent = 0;
            LastModifier = "";
            FabricCert = "";
        }
    }
}
