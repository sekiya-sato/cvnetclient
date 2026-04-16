using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01ShoSh5v2ViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        string m_DgTitle = "商品マスタ : 色サイズ展開";

        [ObservableProperty]
        ShoSh5v2SearchOpt? m_ShoSh5SearchOpt;

        [ObservableProperty]
        ObservableCollection<ShoSh5Item> m_ShoSh5List;
        #endregion

        #region ComboList Variable
        /// <summary>
        /// 自動補充 -> Line3 [0 しない,1 売上基準,9 商品ﾏｽﾀ依存]
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_AutoDistList;

        /// <summary>
        /// 中止 -> Line8 [0 正規,1 中止]
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_StatusList;

        /// <summary>
        /// カタログ -> Line9 [1 新規登録,2 変更,3 取消,9 取消済]
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_CatalogList;

        /// <summary>
        /// 取引情報 -> Line10 [1 新規登録,2 変更,3 取消,9 取消済]
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_TranInfoList;

        /// <summary>
        /// WEB情報 -> Line10 [1 新規登録,2 変更,3 取消,9 取消済]
        /// </summary>
        [ObservableProperty]
        public Dictionary<int, string>? m_WebInfoList;
        #endregion

        #region Class Variable
        private ShoSh5v2Rec JanStyleRec;
        /* 色、サイズ、品番（JAN先頭＋JANコード3）、上代 の段、位置、桁数保存用 */
        private DataTable MakeJan;
        private int pon = 0;
        private DataTable csv00;
        public int resp_code;
        #endregion

        public int OnInit(string[] init_para, int? flg = null)
        {
            resp_code = 0;
            ShoSh5SearchOpt = new ShoSh5v2SearchOpt();
            ShoSh5List = new ObservableCollection<ShoSh5Item>();
            if (init_para.Length < 5) 
            {
                resp_code = -99;
                ClientLib.ExitDialogResult(this, true);
                return resp_code;
            }

            #region ComboList
            var get_dist = new Dictionary<int, string>
            {
                { 0 , "0 しない" },
                { 1 , "1 売上基準" },
                { 9 , "9 商品ﾏｽﾀ依存"}
            };
            AutoDistList = get_dist;

            var get_status = new Dictionary<int, string>
            {
                { 0 , "0 正規" },
                { 1 , "1 中止" }
            };
            StatusList = get_status;

            var get_catalog = new Dictionary<int, string>
            {
                { 1 , "1 新規登録" },
                { 2 , "2 変更" },
                { 3 , "3 取消" },
                { 9 , "9 取消済" }
            };
            CatalogList = get_catalog;

            var get_traninfo = new Dictionary<int, string>
            {
                { 1 , "1 新規登録" },
                { 2 , "2 変更" },
                { 3 , "3 取消" },
                { 9 , "9 取消済" }
            };
            TranInfoList = get_traninfo;

            var get_webinfo = new Dictionary<int, string>
            {
                { 1 , "1 新規登録" },
                { 2 , "2 変更" },
                { 3 , "3 取消" },
                { 9 , "9 取消済" }
            };
            WebInfoList = get_webinfo;
            #endregion

            /* 初期化処理 */
            if (string.IsNullOrEmpty(init_para[5])) init_para[5] = "SIZ";
            OnInitBase(init_para, flg.ToString());

            ShoSh5SearchOpt.ProdCd = init_para[0];
            ShoSh5SearchOpt.OriCost = long.TryParse(init_para[3], out long _oricost) ? _oricost : 0;
            ShoSh5SearchOpt.Cost = long.TryParse(init_para[4], out long _cost) ? _cost : 0;
            /* 商品CD,SEQ_NO,VDATE_UPDATE,元上代,上代,サイズ表示 */
            /* #58636 参照用モードではロックしない */
            int san_flg = 0;
            if (flg != null) {
                if (flg == 3) san_flg = 1;
            }
             
            //if (san_flg == 0) { 
            //    long p_no = long.TryParse(init_para[1], out long _p_no) ? _p_no : 0; 
            //    var ret_aspx = AppData.Http!.AspxSqlExe(DBDef.DB_DML.LOCK, "Master_SHOHIN", p_no, init_para[2], null, null);
            //    if (ret_aspx.Code < 0)
            //    {
            //        resp_code = ret_aspx.Code;
            //        ClientLib.ExitDialogResult(this, true);
            //        return resp_code;
            //    }
            //}

            if (!string.IsNullOrEmpty(init_para[0]))
            {
                string sql_sub = ",Get_Colorname(A.色CD) 色名,get_sizename(A.商品CD,A.サイズCD) サイズ名";
                if (AppData.ClassCvnet.config.ColSizMei == 1) sql_sub = ",A.色名,A.サイズ名";

                string szkbn = init_para[5];
                string sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
                sql_query += ",A.商品CD,A.色CD,A.サイズCD,A.自動配分FLG,A.上代";
                sql_query += sql_sub;

                sql_query += ",A.生産予定数,A.使用FLG";
                /* JANコード1、2、3追加 */
                sql_query += ",A.\"JANコード1\",A.\"JANコード2\",A.\"JANコード3\",A.メモ";

                /****** 20070111 最終修正者追加対応 START ******/
                sql_query += ",A.入力社員CD ||' '||(select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD) 最終修正者";
                /****** 20070111 最終修正者追加対応 END    ******/
                sql_query += ",A.コラボ出力区分";
                sql_query += " from HC$MASTER_SHOHIN_JAN A";
                sql_query += " where A.商品CD=:1";
                sql_query += " order by A.商品CD,A.色CD,A.サイズCD";

                var v_para = new BizArray();
                v_para[0] = init_para[0];
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
                if (ret_csv != null)
                {
                    var list = (from DataRow dr in ret_csv.Rows
                                 select new ShoSh5Item
                                 { 
                                     ColorCD = dr["色CD"].ToString() ?? string.Empty,
                                     SizCD = dr["サイズCD"].ToString() ?? string.Empty,
                                     AutoDistFlg = int.TryParse(dr["自動配分FLG"].ToString(), out var _dist_flg) ? _dist_flg : 0,
                                     AutoDist = _dist_flg == 0 ? 0 : _dist_flg == 1 ? 1 : 9,
                                     Retail = Convert.ToInt64(dr["上代"]),
                                     ColorName = dr["色名"].ToString() ?? string.Empty,
                                     SizName = dr["サイズ名"].ToString() ?? string.Empty,
                                     PlannedQty = int.TryParse(dr["生産予定数"].ToString(), out var _plan_qty) ? _plan_qty : 0,
                                     IsUseFlg = int.TryParse(dr["使用FLG"].ToString(), out var _use_flg) ? _use_flg : 0,
                                     IsUse = _use_flg == 0 ? 0 : 1,
                                     Jancode1 = dr["JANコード1"].ToString() ?? string.Empty,
                                     Jancode2 = dr["JANコード2"].ToString() ?? string.Empty,
                                     Jancode3 = dr["JANコード3"].ToString() ?? string.Empty,
                                     Memo = dr["メモ"].ToString() ?? string.Empty,
                                     CollabOutType = int.TryParse(dr["コラボ出力区分"].ToString(), out var cb) ? cb : 0,
                                     CatalogFlg = cb -  (int)GlobalFunc.RoundDown(cb - 1),
                                     TranInfoFlg = (int)GlobalFunc.RoundDown((cb - GlobalFunc.RoundDown(cb,-2)) / 10),
                                     WebInfoFlg = (int)GlobalFunc.RoundDown(cb / 100)
                                 }).ToList();

                    ShoSh5List = new ObservableCollection<ShoSh5Item>(list);
                    /****** 20070111 最終修正者追加対応 START ******/
                    if (ret_csv.Rows.Count != 0)
                    {
                        ShoSh5SearchOpt.LastModifier = ret_csv.Rows[0][16].ToString();
                        double vdate = double.TryParse(ret_csv.Rows[0][1].ToString(), out var vdate_update) ? vdate_update : 0;
                        ShoSh5SearchOpt.VDateCreate = AppData.ClassSatoo.GetVdate(vdate).ToString();
                    }
                    /****** 20070111 最終修正者追加対応 END    ******/

                    /******************************************************************/
                    /* 20070215追加 */
                    /* JANPRO:JAN生成名称CD */

                    JanStyleRec = new ShoSh5v2Rec();
                    JanStyleRec.renbanflg = AppData.ClassCvnet.config.renbanflg;
                    JanStyleRec.danflg = AppData.ClassCvnet.config.danflg;

                    int addCol = 0;
                    if (JanStyleRec.danflg == 1)
                    {
                        JanStyleRec.janPattern = new DataTable();
                        addCol = 10;
                        for (int i = 0; i < addCol; i++)
                        {
                            JanStyleRec.janPattern.Columns.Add("COL" + i.ToString("00"), typeof(string));
                        }
                        var janrows = AppData.ClassCvnet.config.janpattern.Split("\n");
                        for (int i = 0; i < janrows.Length; i++)
                        {
                            var jancols = janrows[i].Split("@");
                            DataRow row = JanStyleRec.janPattern.NewRow();
                            for (int j = 0; j < jancols.Length; j++)
                            {
                                row["COL" + j.ToString("00")] = jancols[j];
                            }
                            JanStyleRec.janPattern.Rows.Add(row);
                        }

                        JanStyleRec.JanPro = new DataTable();
                        addCol = 5;
                        for (int i = 0; i < addCol; i++)
                        {
                            JanStyleRec.JanPro.Columns.Add("COL" + i.ToString("00"), typeof(string));
                        }
                        for (int i = 0; i < JanStyleRec.janPattern.Rows.Count; i++)
                        {
                            /* 空はｾｯﾄしない 11.05.16 */
                            if (!string.IsNullOrEmpty(JanStyleRec.janPattern.Rows[i][6].ToString()))
                            {
                                DataRow row = JanStyleRec.JanPro.NewRow();
                                row["COL00"] = JanStyleRec.janPattern.Rows[i][3].ToString();
                                row["COL01"] = JanStyleRec.janPattern.Rows[i][6].ToString();
                                row["COL02"] = JanStyleRec.janPattern.Rows[i][7].ToString();
                                row["COL03"] = JanStyleRec.janPattern.Rows[i][8].ToString();
                                row["COL04"] = JanStyleRec.janPattern.Rows[i][9].ToString();
                                JanStyleRec.JanPro.Rows.Add(row);
                            }
                        }

                        /* barPatternのセット */
                        JanStyleRec.barPattern = new DataTable();
                        var barrows = AppData.ClassCvnet.config.barpattern.Split("\n");
                        addCol = 8;
                        for (int i = 0; i < addCol; i++)
                        {
                            JanStyleRec.janPattern.Columns.Add("COL" + i.ToString("00"), typeof(string));
                        }
                        for (int i = 0; i < barrows.Length; i++)
                        {
                            var barcols = barrows[i].Split("@");
                            DataRow row = JanStyleRec.barPattern.NewRow();
                            for (int j = 0; j < barcols.Length; j++)
                            {
                                row["COL" + j.ToString("00")] = barcols[j];
                            }
                            JanStyleRec.barPattern.Rows.Add(row);
                        }

                        var barrows2 = JanStyleRec.barPattern.Rows;
                        for (int i = 0; i < barrows2.Count; i++)
                        {
                            DataRow row = JanStyleRec.JanPro.NewRow();
                            row["COL00"] = JanStyleRec.barPattern.Rows[i][1].ToString();
                            row["COL01"] = JanStyleRec.barPattern.Rows[i][3].ToString();
                            row["COL02"] = JanStyleRec.barPattern.Rows[i][4].ToString();
                            row["COL03"] = JanStyleRec.barPattern.Rows[i][5].ToString();
                            row["COL04"] = JanStyleRec.barPattern.Rows[i][6].ToString();
                            JanStyleRec.JanPro.Rows.Add(row);
                        }

                        DataView dv = JanStyleRec.JanPro.DefaultView;
                        dv.Sort = "COL01 DESC, COL02 DESC";
                        JanStyleRec.JanPro = dv.ToTable();
                    }

                    /* 行が空の場合は削除 */
                    for (int i = JanStyleRec.JanPro.Rows.Count - 1; i >= 0; i--)
                    {
                        if (string.IsNullOrEmpty(JanStyleRec.JanPro.Rows[i][1]?.ToString()))
                        {
                            JanStyleRec.JanPro.Rows.RemoveAt(i);
                        }
                    }

                    /* JAN作成用のCSV用意 */
                    string dname = "";
                    if (JanStyleRec.danflg == 1) dname = "連番";
                    else dname = "品番";

                    /* 原価FLG追加 20070305 */
                    addCol = 4;
                    MakeJan = new DataTable();
                    for (int i = 0; i < addCol; i++)
                    {
                        MakeJan.Columns.Add("COL" + i.ToString("00"), typeof(string));
                    }
                    for (int i = 0; i < 6; i++)
                    {
                        DataRow row = MakeJan.NewRow();
                        switch (i)
                        { 
                            case 0:
                                row["COL00"] = dname;
                                row["COL01"] = AppData.ClassCvnet.SysHhtMst.Rows[0][9].ToString();
                                row["COL02"] = AppData.ClassCvnet.SysHhtMst.Rows[0][10].ToString();
                                row["COL03"] = AppData.ClassCvnet.SysHhtMst.Rows[0][11].ToString();
                                break;
                            case 1:
                                row["COL00"] = "色";
                                row["COL01"] = AppData.ClassCvnet.SysHhtMst.Rows[0][12].ToString();
                                row["COL02"] = AppData.ClassCvnet.SysHhtMst.Rows[0][13].ToString();
                                row["COL03"] = AppData.ClassCvnet.SysHhtMst.Rows[0][14].ToString();
                                break;
                            case 2:
                                row["COL00"] = "サイズ";
                                row["COL01"] = AppData.ClassCvnet.SysHhtMst.Rows[0][15].ToString();
                                row["COL02"] = AppData.ClassCvnet.SysHhtMst.Rows[0][16].ToString();
                                row["COL03"] = AppData.ClassCvnet.SysHhtMst.Rows[0][17].ToString();
                                break;
                            case 3:
                                row["COL00"] = "上代";
                                row["COL01"] = AppData.ClassCvnet.SysHhtMst.Rows[0][18].ToString();
                                row["COL02"] = AppData.ClassCvnet.SysHhtMst.Rows[0][19].ToString();
                                row["COL03"] = AppData.ClassCvnet.SysHhtMst.Rows[0][20].ToString();
                                break;
                            case 4:
                                row["COL00"] = "シリアル";
                                row["COL01"] = AppData.ClassCvnet.SysHhtMst.Rows[0][21].ToString();
                                row["COL02"] = AppData.ClassCvnet.SysHhtMst.Rows[0][22].ToString();
                                row["COL03"] = AppData.ClassCvnet.SysHhtMst.Rows[0][23].ToString();
                                break;
                            case 5:
                                if (AppData.ClassCvnet.SysHhtMst.Columns.Count > 37)
                                {
                                    row["COL00"] = "原価FLG";
                                    row["COL01"] = AppData.ClassCvnet.SysHhtMst.Rows[0][37].ToString();
                                    row["COL02"] = AppData.ClassCvnet.SysHhtMst.Rows[0][38].ToString();
                                    row["COL03"] = AppData.ClassCvnet.SysHhtMst.Rows[0][39].ToString();
                                }
                                else
                                {
                                    row["COL00"] = "原価FLG";
                                    row["COL01"] = "0";
                                    row["COL02"] = "0";
                                    row["COL03"] = "0";
                                }

                                break;
                            default:
                                break;
                        }
                        MakeJan.Rows.Add(row);
                    }
                    if (JanStyleRec.danflg == 1) {
                        for (int i = 0; i < JanStyleRec.JanPro.Rows.Count; i++)
                        {
                            DataRow row = MakeJan.NewRow();
                            row["COL00"] = JanStyleRec.JanPro.Rows[i][0].ToString();
                            row["COL01"] = JanStyleRec.JanPro.Rows[i][1].ToString();
                            row["COL02"] = JanStyleRec.JanPro.Rows[i][2].ToString();
                            row["COL03"] = JanStyleRec.JanPro.Rows[i][3].ToString();
                            MakeJan.Rows.Add(row);
                        }
                    }

                    DataView dv2 = MakeJan.DefaultView;
                    dv2.Sort = "COL01 DESC, COL02 DESC";
                    MakeJan = dv2.ToTable();

                    /* 参照用 */
                    if (flg != null)
                    {
                        if (flg == 3)
                        {
                            ShoSh5SearchOpt.DesideVis = Visibility.Collapsed;
                            DgTitle += "（参照用）";
                        }
                    }
                }
            }
            return resp_code;
        }

        #region Button Events
        /// <summary>
        /// 色サイズ展開補助
        /// </summary>
        [RelayCommand]
        private void DoColSizDeploy()
        {
            if (ShoSh5SearchOpt == null) return;
            var ret_csv = AppData.Http?.AspxSqlQuery("select 名称CD, 名称 from hc$master_meisho where 名称区分='COL' order by 名称CD");
            if (ret_csv == null) return;
            var list = (from DataRow dr in ret_csv.Rows
                        select new ShoSh5ItemModel
                        {
                            IsSelected = false,
                            Code = dr["名称CD"].ToString() ?? string.Empty,
                            Name = dr["名称"].ToString() ?? string.Empty
                        }).ToList();
            int mid = list.Count / 2;
            ShoSh5SearchOpt.SelectedColor1 = new ObservableCollection<ShoSh5ItemModel>(list.Take(mid));
            ShoSh5SearchOpt.SelectedColor2 = new ObservableCollection<ShoSh5ItemModel>(list.Skip(mid));

            ret_csv = AppData.Http?.AspxSqlQuery("select 名称CD, 名称 from hc$master_meisho where 名称区分='" + para[5] + "' order by 名称CD");
            if (ret_csv == null) return;
            var list2 = (from DataRow dr in ret_csv.Rows
                         select new ShoSh5ItemModel
                         {
                             IsSelected = false,
                             Code = dr["名称CD"].ToString() ?? string.Empty,
                             Name = dr["名称"].ToString() ?? string.Empty
                         }).ToList();
            ShoSh5SearchOpt.SelectedSiz = new ObservableCollection<ShoSh5ItemModel>(list2);
        }

        /// <summary>
        /// 色サイズ\n展開作成
        /// </summary>
        [RelayCommand]
        private void DoAddColSiz()
        {
            if (ShoSh5SearchOpt == null) return;
            int col1_cnt = ShoSh5SearchOpt.SelectedColor1.Select(x => x.IsSelected == true).Count();
            int col2_cnt = ShoSh5SearchOpt.SelectedColor2.Select(x => x.IsSelected == true).Count();
            int siz_cnt = ShoSh5SearchOpt.SelectedSiz.Select(x => x.IsSelected == true).Count();

            if (col1_cnt < 0 && col2_cnt < 0 || siz_cnt < 0) return;
            if (ShoSh5SearchOpt.IsAdded == false) ShoSh5List.Clear(); /* 追加/上書きチェック */

            for (int i = 0; i < ShoSh5SearchOpt.SelectedColor1.Count; i++)
            {
                if (ShoSh5SearchOpt.SelectedColor1[i].IsSelected == true)
                {
                    for (int j = 0; j < ShoSh5SearchOpt.SelectedSiz.Count; j++)
                    {
                        if (ShoSh5SearchOpt.SelectedSiz[j].IsSelected == true) 
                        {
                            int flg = 0;
                            /* 同じのチェック */
                            if (ShoSh5SearchOpt.IsAdded == true && ShoSh5List.Count > 0) 
                            {
                                for (int z = 0; z < ShoSh5List.Count; z++)
                                {
                                    if (ShoSh5List[z].ColorCD == ShoSh5SearchOpt.SelectedColor1[i].Code && ShoSh5List[z].SizCD == ShoSh5SearchOpt.SelectedSiz[j].Code)
                                    {
                                        flg = 1; break;
                                    }
                                }
                            }
                            if (flg == 0)
                            {
                                ShoSh5Item new_row = new ShoSh5Item();
                                new_row.ColorCD = ShoSh5SearchOpt.SelectedColor1[i].Code;
                                new_row.ColorName = ShoSh5SearchOpt.SelectedColor1[i].Name;
                                new_row.SizCD = ShoSh5SearchOpt.SelectedSiz[j].Code;
                                new_row.SizName = ShoSh5SearchOpt.SelectedSiz[j].Name;
                                new_row.IsUse = 0;
                                new_row.AutoDist = 9;
                                ShoSh5List.Add(new_row);
                            }
                        }
                    }
                }
            }
            /* 面倒なので２回繰り返す */
            for (int i = 0; i < ShoSh5SearchOpt.SelectedColor2.Count; i++)
            {
                if (ShoSh5SearchOpt.SelectedColor2[i].IsSelected == true)
                {
                    for (int j = 0; j < ShoSh5SearchOpt.SelectedSiz.Count; j++)
                    {
                        if (ShoSh5SearchOpt.SelectedSiz[j].IsSelected == true)
                        {
                            int flg = 0;
                            /* 同じのチェック */
                            if (ShoSh5SearchOpt.IsAdded == true && ShoSh5List.Count > 0)
                            {
                                for (int z = 0; z < ShoSh5List.Count; z++)
                                {
                                    if (ShoSh5List[z].ColorCD == ShoSh5SearchOpt.SelectedColor2[i].Code && ShoSh5List[z].SizCD == ShoSh5SearchOpt.SelectedSiz[j].Code)
                                    {
                                        flg = 1; break;
                                    }
                                }
                            }
                            if (flg == 0)
                            {
                                ShoSh5Item new_row = new ShoSh5Item();
                                new_row.ColorCD = ShoSh5SearchOpt.SelectedColor2[i].Code;
                                new_row.ColorName = ShoSh5SearchOpt.SelectedColor2[i].Name;
                                new_row.SizCD = ShoSh5SearchOpt.SelectedSiz[j].Code;
                                new_row.SizName = ShoSh5SearchOpt.SelectedSiz[j].Name;
                                new_row.IsUse = 0;
                                new_row.AutoDist = 9;
                                ShoSh5List.Add(new_row);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 上代一括クリア
        /// </summary>
        [RelayCommand]
        private void DoClearAllPrice()
        {
            if (ShoSh5List?.Count == 0) return;
            if (MessageBox.Show("色サイズマスタの上代を一括クリアし\n商品マスタの上代を優先しますか?", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                for (int i = 0; i < ShoSh5List?.Count; i++) {
                    ShoSh5List[i].Retail = 0;
                }
            }
        }

        /// <summary>
        /// 中止一括ON/OFF
        /// </summary>
        [RelayCommand]
        private void DoBatchCancelToggle()
        {
            if (ShoSh5List?.Count == 0) return;
            if (MessageBox.Show("色サイズマスタの中止FLGを一括ON/OFFしますか?\n※ ON→OFF/OFF→ON", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                for (int i = 0; i < ShoSh5List?.Count; i++)
                {
                    if (ShoSh5List[i].IsUse == 0)
                        ShoSh5List[i].IsUse = 1;
                    else
                        ShoSh5List[i].IsUse = 0;
                }
            }
        }

        /// <summary>
        /// 配分一括マスタ依存
        /// </summary>
        [RelayCommand]
        private void DoApplyMasterAll()
        {
            if (ShoSh5List?.Count == 0) return;
            if (MessageBox.Show("色サイズマスタの配分設定をを一括クリアし\n商品マスタの配分FLGを優先しますか?", "確認", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                for (int i = 0; i < ShoSh5List?.Count; i++)
                {
                    ShoSh5List[i].AutoDist = 9;
                }
            }
        }

        /// <summary>
        /// 確定(F6)
        /// </summary>
        [RelayCommand]
        private void DoConfirm()
        {
            int Error_Check = OnCheckError();
            if (Error_Check < 0)
            {
                if (Error_Check == -2) MessageBox.Show("色サイズCDが重複しています！", "エラー", MessageBoxButton.OK);
                else if (Error_Check == -3) MessageBox.Show("JANコード3が重複しています！", "エラー", MessageBoxButton.OK);
                else MessageBox.Show("各コードを正しく入力して下さい！", "エラー", MessageBoxButton.OK);
                return;
            }
            csv00 = new DataTable(); 
            for (int i = 0; i < 13; i++)
            {
                csv00.Columns.Add("COL" + i.ToString("00"), typeof(string));
            }
            for (int i = 0; i < ShoSh5List?.Count; i++)
            {
                DataRow row = csv00.NewRow();

                var cb = ShoSh5List[i].CatalogFlg + (ShoSh5List[i].TranInfoFlg * 10);
                /****** 20070111 最終修正者追加対応 START ******/ /* コラボデータ一番後ろに */
                //row["COL00"]
            }
        }

        /// <summary>
        /// 戻る(ESC)
        /// </summary>
        [RelayCommand]
        private void DoExit()
        {
            long p_no = long.TryParse(para[1], out long _p_no) ? _p_no : 0;
            var ret_aspx = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UNLOCK, "Master_SHOHIN", p_no, para[2], null, null);
            ClientLib.ExitDialogResult(this, true);
        }
        #endregion

        #region Function
        /* エラーチェック　06.07.19 */
        private int OnCheckError()
        {
            int v_err = 0;
            if (ShoSh5List.Count <= 0) return v_err;
            string sql_query = "";
            var v_para = new BizArray();

            /* 2020.02.20 #53920_重複チェック */
            string sql_query2 = "";
            string sql_query3 = "";

            for (int i = 0; i < ShoSh5List.Count; i++)
            {
                /* 2017.04.06 条件を修正 */
                /* PKG共通エラーチェック */

                int row_col = MakeJan.AsEnumerable()
                                     .Select((row, i) => new { row, i })
                                     .FirstOrDefault(x => x.row[0]?.ToString() == "色")?.i ?? -1;
                int coldata01 = int.TryParse(MakeJan.Rows[row_col][3].ToString(), out int _col01) ? _col01 : 0;
                if (row_col >= 0 && coldata01 > 0 && !string.IsNullOrEmpty(MakeJan.Rows[row_col][3].ToString()))
                {
                    if (ShoSh5List[i].ColorCD.Length.ToString() != MakeJan.Rows[row_col][3].ToString())
                    {
                        ShoSh5List[i].ColorBgColor = Brushes.Red;
                        v_err = -1;
                    }
                }
                var row_siz = MakeJan.AsEnumerable()
                                     .Select((row, i) => new { row, i })
                                     .FirstOrDefault(x => x.row[0]?.ToString() == "サイズ")?.i ?? -1;
                int coldata02 = int.TryParse(MakeJan.Rows[row_siz][3].ToString(), out int _col02) ? _col02 : 0;
                if (row_siz >= 0 && coldata02 > 0 && !string.IsNullOrEmpty(MakeJan.Rows[row_siz][3].ToString()))
                {
                    if (ShoSh5List[i].SizCD.Length.ToString() != MakeJan.Rows[row_col][3].ToString())
                    {
                        ShoSh5List[i].SizBgColor = Brushes.Red;
                        v_err = -1;
                    }
                }

                if (i != 0)
                    sql_query += " union all select ";
                else
                    sql_query += "select ";

                sql_query += (i + 1) + ", ";
                sql_query += "nvl((select 0 from HC$master_meisho where 名称区分='COL' and 名称CD ='" + ShoSh5List[i].ColorCD.Trim() + "'),-1) 色FLG,";
                /* 2021.02.07 #54134_サイズ名、空名有効に変更 */
                sql_query += "nvl((select 0 from HC$master_meisho where 名称区分=nvl((select 商品サイズ区分 from HC$MASTER_SHOHIN where 商品CD='" + ShoSh5SearchOpt?.ProdCd + "'),'.') and 名称CD='" + ShoSh5List[i].SizCD.Trim() + "'),-1) サイズFLG";
                sql_query += " from dual ";

                /* 2020.02.20 #53920_重複チェック */
                if (sql_query2 != "") sql_query2 += " union all ";
                sql_query2 += "select '" + ShoSh5List[i].ColorCD.Trim() + "' 色CD,'" + ShoSh5List[i].SizCD.Trim() + "' サイズCD from dual";
                if (!string.IsNullOrEmpty(ShoSh5List[i].Jancode3))
                {
                    if (sql_query3 != "") sql_query3 += " union all ";
                    sql_query3 += "select '" + ShoSh5List[i].Jancode3.Trim() + "' JANコード3 from dual";
                }
            }

            var wrk_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
            if (wrk_csv != null) {
                for (int i = 0; i < ShoSh5List.Count; i++)
                {
                    if (wrk_csv.Rows[i][1].ToString() != "0")
                    {
                        ShoSh5List[i].ColorBgColor = Brushes.Red;
                        v_err = -1;
                    }
                    else ShoSh5List[i].ColorBgColor = Brushes.White;
                    if (wrk_csv.Rows[i][2].ToString() != "0")
                    {
                        ShoSh5List[i].SizBgColor = Brushes.Red;
                        v_err = -1;
                    }
                    else ShoSh5List[i].SizBgColor = Brushes.White;
                }
            }

            /* 2020.02.20 #53920_重複チェック */
            wrk_csv = AppData.Http?.AspxSqlQuery("select max(max_cnt) max_cnt from (select sum(1) over (partition by 色CD, サイズCD) max_cnt from (" + sql_query2 + "))");
            if (wrk_csv?.Rows[0][0].ToString() != "1" && v_err == 0) v_err = -2;
            if (!string.IsNullOrEmpty(sql_query3))
            {
                wrk_csv = AppData.Http?.AspxSqlQuery("select max(max_cnt) max_cnt from (select sum(1) over (partition by JANコード3) max_cnt from (" + sql_query3 + "))");
                if (wrk_csv?.Rows[0][0].ToString() != "1" && v_err == 0) v_err = -3;
            }
            return v_err;
        }

        /* 2020.12.28 #58942対応追加（伝票存在チェック） */
        private int CheckDen(string[] v_para)
        {
            string sql_str = ""
                    + " select 1 c from dual"
                    + " where "
                        + "exists (select 'X' from hc$tran_tori1 t1,HC$MASTER_SHOHIN_JAN j "
                                    + " where t1.商品CD=j.商品CD and t1.色CD=j.色CD and t1.サイズCD=j.サイズCD"
                                    + " and t1.商品CD=:1 and t1.色CD=:2 and t1.サイズCD=:3 and j.JANコード1=:4 and t1.伝票処理区分 not in (6,7))"
                    + "";
            var ret_csv = AppData.Http?.AspxSqlQuery(sql_str, v_para);
            if (ret_csv?.Rows.Count > 0) return -1;
            return 0;
        }
        #endregion
    }

    public class ShoSh5v2Rec
    {
        /* 070215追加 */
        public int danflg { get; set; }

        public int renbanflg { get; set; }

        public DataTable janPattern {  get; set; }

        public DataTable barPattern { get; set; }

        public DataTable JanPro {  get; set; }

        public ShoSh5v2Rec()
        {
            danflg = 0;
            renbanflg = 0;
            janPattern = new DataTable();
            barPattern = new DataTable();
            JanPro = new DataTable();
        }
    }

    public class ShoSh5ItemModel
    { 
        public bool IsSelected { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }

        public ShoSh5ItemModel()
        {
            Code = string.Empty;
            Name = string.Empty;
            IsSelected = false;
        }
    }

    public partial class ShoSh5v2SearchOpt : ObservableObject
    {
        /// <summary>
        /// 商品CD (Text1)
        /// </summary>
        [ObservableProperty]
        string m_ProdCd;

        /// <summary>
        /// 元上代 (Text2)
        /// </summary>
        [ObservableProperty]
        long m_OriCost;

        /// <summary>
        /// 上代 (Text3)
        /// </summary>
        [ObservableProperty]
        long m_Cost;

        /// <summary>
        /// 最終修正者 (Text4)
        /// </summary>
        [ObservableProperty]
        string m_LastModifier;

        /// <summary>
        /// 追加 Checkbox (CheckBox1)
        /// </summary>
        [ObservableProperty]
        bool m_IsAdded;

        /// <summary>
        /// 追加 Checkbox (CheckBox1) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_IsAddedVis;

        /// <summary>
        /// ColorList1 Selected (ListBox1)
        /// </summary>
        [ObservableProperty] 
        ObservableCollection<ShoSh5ItemModel> m_SelectedColor1;

        /// <summary>
        /// ColorList2 Selected (ListBox3)
        /// </summary>
        [ObservableProperty]
        ObservableCollection<ShoSh5ItemModel> m_SelectedColor2;

        /// <summary>
        /// SizList Selected (ListBox2)
        /// </summary>
        [ObservableProperty]
        ObservableCollection<ShoSh5ItemModel> m_SelectedSiz;

        /// <summary>
        /// 作成日 (Text5)
        /// </summary>
        [ObservableProperty]
        string m_VDateCreate;

        /// <summary>
        /// 確定(F6) (Bt_Deside) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_DesideVis; 

        public ShoSh5v2SearchOpt()
        { 
            ProdCd = string.Empty;
            OriCost = 0;
            Cost = 0;
            LastModifier = string.Empty;
            IsAdded = true;
            IsAddedVis = Visibility.Collapsed;
            SelectedColor1 = new ObservableCollection<ShoSh5ItemModel>();
            SelectedColor2 = new ObservableCollection<ShoSh5ItemModel>();
            SelectedSiz = new ObservableCollection<ShoSh5ItemModel>();
            DesideVis = Visibility.Visible;
        }
    }

    public partial class ShoSh5Item : ObservableObject
    {
        /// <summary>
        /// 色CD (Line1)
        /// </summary>
        [ObservableProperty]
        string m_ColorCD;

        /// <summary>
        /// 色CD (Line1) Background Color
        /// </summary>
        [ObservableProperty]
        Brush m_ColorBgColor;

        /// <summary>
        /// サイズCD (Line2)
        /// </summary>
        [ObservableProperty]
        string m_SizCD;

        /// <summary>
        /// サイズCD (Line2) Background Color
        /// </summary>
        [ObservableProperty]
        Brush m_SizBgColor;

        /// <summary>
        /// 自動配分FLG
        /// </summary>
        [ObservableProperty]
        int m_AutoDistFlg;

        /// <summary>
        /// 自動配分表示 (Line3)
        /// </summary>
        [ObservableProperty]
        int m_AutoDist;

        /// <summary>
        /// 上代 (Line4)
        /// </summary>
        [ObservableProperty]
        long m_Retail;

        /// <summary>
        /// 色名 (mei1)
        /// </summary>
        [ObservableProperty]
        string m_ColorName;

        /// <summary>
        /// 色名 ReadOnly
        /// </summary>
        [ObservableProperty]
        bool m_ColorNameRO;

        /// <summary>
        /// サイズ名 (mei2)
        /// </summary>
        [ObservableProperty]
        string m_SizName;

        /// <summary>
        /// サイズ名 ReadOnly
        /// </summary>
        [ObservableProperty]
        bool m_SizNameRO;

        /// <summary>
        /// 生産予定数 (Line7)
        /// </summary>
        [ObservableProperty]
        int m_PlannedQty;

        /// <summary>
        /// 生産予定数 Column Width
        /// </summary>
        [ObservableProperty]
        int m_PlanQtyWidth;
         
        /// <summary>
        /// 使用FLG
        /// </summary>
        [ObservableProperty]
        int m_IsUseFlg;

        /// <summary>
        /// 使用表示 (Line8)
        /// </summary>
        [ObservableProperty]
        int m_IsUse;

        /// <summary>
        /// コラボ出力区分
        /// </summary>
        [ObservableProperty]
        int m_CollabOutType;

        /// <summary>
        /// カタログ (Line9)
        /// </summary>
        [ObservableProperty]
        int m_CatalogFlg;

        /// <summary>
        /// カタログ Width
        /// </summary>
        [ObservableProperty]
        int m_CatalogWidth;

        /// <summary>
        /// 取引情報 (Line10)
        /// </summary>
        [ObservableProperty]
        int m_TranInfoFlg;

        /// <summary>
        /// 取引情報 Width
        /// </summary>
        [ObservableProperty]
        int m_TranInfoWidth;

        /// <summary>
        /// WEB情報 (Line11)
        /// </summary>
        [ObservableProperty]
        int m_WebInfoFlg;

        /// <summary>
        /// WEB情報 Width
        /// </summary>
        [ObservableProperty]
        int m_WebInfoWidth;

        /// <summary>
        /// JANコード1 (Line21)
        /// </summary>
        [ObservableProperty]
        string m_Jancode1;

        /// <summary>
        /// JANコード1 ReadOnly
        /// </summary>
        [ObservableProperty]
        bool m_Jancode1RO;

        /// <summary>
        /// JANコード2 (Line22)
        /// </summary>
        [ObservableProperty]
        string m_Jancode2;

        /// <summary>
        /// JANコード2 ReadOnly
        /// </summary>
        [ObservableProperty]
        bool m_Jancode2RO;

        /// <summary>
        /// JANコード3 (Line23)
        /// </summary>
        [ObservableProperty]
        string m_Jancode3;

        /// <summary>
        /// メモ (Line24)
        /// </summary>
        [ObservableProperty]
        string m_Memo;

        public ShoSh5Item()
        { 
            ColorCD = string.Empty;
            ColorBgColor = Brushes.White;
            SizCD = string.Empty;
            SizBgColor = Brushes.White;
            AutoDistFlg = 0;
            AutoDist = 0;
            Retail = 0;
            ColorName = string.Empty;
            ColorNameRO = true;
            SizName = string.Empty;
            SizNameRO = true;
            PlannedQty = 0;
            PlanQtyWidth = 0;
            IsUseFlg = 0;
            IsUse = 0;
            CatalogFlg = 0;
            CatalogWidth = 0;
            TranInfoFlg = 0;
            TranInfoWidth = 0;
            WebInfoFlg = 0;
            WebInfoWidth = 0;
            Jancode1 = string.Empty;
            Jancode1RO = true;
            Jancode2 = string.Empty;
            Jancode2RO = true;
            Jancode3 = string.Empty;
            Memo = string.Empty; 
        }
    }
}
