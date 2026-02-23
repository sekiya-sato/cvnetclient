using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using static System.Net.Mime.MediaTypeNames;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp12ViewModel : BaseViewModel
    {
        #region Variables
        [ObservableProperty]
        public int? m_SelectedTabIndex;

        [ObservableProperty]
        public string? m_Mess1;

        [ObservableProperty]
        public string? m_Mess2;

        [ObservableProperty]
        public Inp12SearchOpt m_Inp12SearchOpt;

        [ObservableProperty]
        public ObservableCollection<SelOrderTran> m_OrderList;

        [ObservableProperty]
        public SelOrderTran m_SelOrderItem;

        [ObservableProperty]
        public int? m_PrdToggle = 0;
        #endregion

        #region Private Variable
        private int Chg_flg = 0;
        /* デフォルトをイメージボタン形式にしてみる */
        private int call_dlg = 9;
        private int max_col = 84;	/* 品番印刷用 */
        private int CSV_flg = 0;

        private BizArray col_list; /* Insert,Update時の項目名リスト */
        private BizArray col_list2; /* 明細の項目名リスト */
        private string sql_collist = "関連伝票NO,在庫計上日,納品日,取引区分,入力社員CD,倉庫CD," 
                                   + "取引先CD1,掛率1,外税対象金額,数量合計,明細金額合計,"
	                               + "内税消費税,外税消費税,上代合計,下代合計,メモ,掛率2,伝票処理区分,MOD_SEQ,関連伝票NO2,展示会CD"
	                               + ",手入力伝票NO,納品先CD,担当者CD";
        /* 完了FLG追加 2007.11.27 */
        /* 納品日追加 2008.09.26 */
        /* 明細承認FLG追加 2014/12/09 */
        private string sql_collist2 = "明細取引区分,商品CD,色CD,サイズCD,明細名称,数量,単価,金額,内税消費税,外税消費税,"
                                    + "上代単価,上代金額,下代単価,下代金額,明細メモ,消費税計算方法"
                                    + ",商品シリアル,関連伝票NO,関連伝票行NO,JANCODE,原価FLG,完了FLG"
                                    + ",納品日,明細承認FLG";
        private BizArray? param1;
        private BizArray? param2;
        #endregion

        #region ComboList Variable
        /// <summary>
        /// 取引区分 List
        /// </summary>
        [ObservableProperty]
        public Dictionary<string, string>? m_TranCatList;
        #endregion

        public void OnInit(object? init_para = null, string? init_flg = null)
        {
            OnInitBase(init_para, init_flg);
            Inp12SearchOpt = new Inp12SearchOpt();

            col_list = new BizArray();
            var wrk_list = sql_collist.Split(",");
            foreach (var wrk in wrk_list)
            { 
                col_list.Add(wrk);
            }
            col_list2 = new BizArray();
            var wrk2_list = sql_collist2.Split(",");
            foreach (var wrk2 in wrk2_list)
            { 
                col_list2.Add(wrk2);
            }

            #region Set ComboList
            var comboItem = AppData.ClassCvnet.comboItem00;

            var get_cats =  comboItem.ComboItem_00<string>("商品受注区分");
            var set_cats = new Dictionary<string, string>();
            set_cats.Add("00", "00");
            foreach (var cat in get_cats)
            {
                set_cats.Add(cat.Key, cat.Value);
            }
            set_cats.Add("99","99");
            TranCatList = set_cats;
            Inp12SearchOpt.TranStartCat = TranCatList.FirstOrDefault().Key;
            Inp12SearchOpt.TranEndCat = TranCatList.LastOrDefault().Key;

            var status = new Dictionary<int, string>
            {
                { 0 ,"0 未完" },
                { 1 ,"1 完了" }
            };
            StatusList = status;
            #endregion
        }

        #region Dialog Search
        [RelayCommand]
        public void SelCustDest01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.CustStartDest = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp12SearchOpt.CustEndDest = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelCustDest02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.CustEndDest = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelStoreCd01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.StoreStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp12SearchOpt.StoreEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelStoreCd02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.StoreEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelProdCd01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.ProdStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp12SearchOpt.ProdEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelProdCd02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.ProdEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelExhibitCd01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.ExhibitStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp12SearchOpt.ExhibitEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelExhibitCd02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.ExhibitEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelInpCd01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.InpStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp12SearchOpt.InpEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelInpCd02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12SearchOpt != null)
            {
                Inp12SearchOpt.InpEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        #endregion

        #region Events
        [RelayCommand]
        public void SelPrdToggle()
        {
            if (PrdToggle == 0) PrdToggle = 1;
            else PrdToggle = 0;
        }

        [RelayCommand]
        public void DoSearch()
        {
            /* パラメータ生成 */
            CreatePara();

            /* 20210205 ページ表示機能 */
            OnQuery(param1.ToArray(), param2.ToArray(), "");
        }

        [RelayCommand]
        public void DoPrevButton()
        {
            if (OrderList.Count == 0) return;

            /* パラメータ生成 */
            CreatePara();

            /* 20210205 ページ表示機能 */
            SelOrderTran oder_tran = OrderList.FirstOrDefault();
            if (oder_tran != null) 
                OnQuery(param1.ToArray(), param2.ToArray(), oder_tran.SeqNo.ToString(), "asc");
        }

        [RelayCommand]
        public void DoNextButton()
        {
            if (OrderList.Count == 0) return;

            /* パラメータ生成 */
            CreatePara();

            SelOrderTran oder_tran = OrderList.LastOrDefault();
            if (oder_tran != null)
                OnQuery(param1.ToArray(), param2.ToArray(), oder_tran.SeqNo.ToString());
        }

        [RelayCommand]
        private void RowDoubleClick(SelOrderTran item)
        {
            if (item == null) item = SelOrderItem;
            if (item == null) return;
            if (Inp12DetailOpt == null) Inp12DetailOpt = new Inp12DetailOpt();
            double created = double.TryParse(item.VdateCreate.ToString(), out created) ? created : 0;
            Inp12DetailOpt.CreatedAt = AppData.ClassSatoo.GetVdate(created).ToString();
            double updated = double.TryParse(item.VdateUpdate.ToString(), out updated) ? updated : 0;
            Inp12DetailOpt.UpdatedAt = AppData.ClassSatoo.GetVdate(updated).ToString(); ;
            Inp12DetailOpt.RelatedNo = item.RelatedSlipNo.ToString(); 
            Inp12DetailOpt.OrderDate = DateTime.ParseExact(item.InvCountDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            Inp12DetailOpt.DeliverDate = DateTime.ParseExact(item.DeliverDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            Inp12DetailOpt.TranCate = item.TranCate;
            Inp12DetailOpt.InpCd = new BtListHelper(item.InpStaffCD, item.InpStaffName);
            Inp12DetailOpt.StoreCd = new BtListHelper(item.StoreCD, item.StoreName);
            Inp12DetailOpt.CustDest = new BtListHelper(item.ClientCD1, item.TradeName);
            Inp12DetailOpt.MarkupRate = item.MarkupRate1;
            Inp12DetailOpt.Num = int.TryParse(item.TotalNum.ToString(), out var num) ? num : 0;
            Inp12DetailOpt.RetailPrice = int.TryParse(item.TotalRetail.ToString(), out var retail) ? retail : 0;
            Inp12DetailOpt.WholesalePrice = int.TryParse(item.TotalWholesale.ToString(), out var whole) ? whole : 0;
            Inp12DetailOpt.Memo = item.Memo;
            Inp12DetailOpt.RelatedNo2 = item.RelatedSlipNo2.ToString();
            Inp12DetailOpt.ExhibitCd = new BtListHelper(item.ExhibitCD, item.ExhibitName);
            Inp12DetailOpt.ManualInpNo = item.ManualInpNo;
            Inp12DetailOpt.SalesRep = new BtListHelper(item.StaffCD, item.StaffName);
            Inp12DetailOpt.SlipNo = item.SeqNo.ToString();
            SelectedTabIndex = 1;

            /* 伝票NOが無い場合 */
            if (string.IsNullOrEmpty(item.SeqNo.ToString())) return;

            BizArray wrk_para = new BizArray();
            wrk_para[0] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd"); /* 在庫計上日 */
            wrk_para[1] = item.StoreCD; /* 倉庫 */
            wrk_para[2] = item.SeqNo.ToString();
            wrk_para[3] = item.TranCate;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0)
                wrk_para[0] = "19000101";

            var ret_csv = OnQueryDetail(wrk_para);
            var list = (from DataRow dr in ret_csv.Rows
                        select new Inp12DetailItem
                        {
                            SeqNo = Convert.ToInt32(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDouble(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDouble(dr["VDATE_UPDATE"]),
                            TranCate = Convert.ToInt16(dr["明細取引区分"]),
                            ProdCD = dr["商品CD"].ToString() ?? string.Empty,
                            ColorCD = dr["色CD"].ToString() ?? string.Empty,
                            SizeCD = dr["サイズCD"].ToString() ?? string.Empty,
                            DetailName = dr["明細名称"].ToString() ?? string.Empty,
                            Num = int.TryParse(dr["数量"].ToString(), out var num) ? num : 0,
                            UnitPrice = int.TryParse(dr["単価"].ToString(), out var unit_price) ? unit_price : 0,
                            TotalAmount = int.TryParse(dr["金額"].ToString(), out var amount) ? amount : 0,
                            TaxIncluded = Convert.ToInt64(dr["内税消費税"]),
                            TaxExcluded = Convert.ToInt64(dr["外税消費税"]),
                            RetailUnitPrice = int.TryParse(dr["上代単価"].ToString(), out var retail_unit) ? retail_unit : 0,
                            RetailAmount = int.TryParse(dr["上代金額"].ToString(), out var retail_amt) ? retail_amt : 0,
                            WholesalesUnit = int.TryParse(dr["下代単価"].ToString(), out var whole_unit) ? whole_unit : 0,
                            WholesalesAmount = int.TryParse(dr["下代金額"].ToString(), out var whole_amt) ? whole_amt : 0,
                            DetailMemo = dr["明細メモ"].ToString() ?? string.Empty,
                            TaxCalcMethod = Convert.ToInt16(dr["消費税計算方法"]),
                            ProdSerial = dr["商品シリアル"].ToString() ?? string.Empty,
                            RelatedSlipNo = Convert.ToInt64(dr["関連伝票NO"]),
                            RelatedSlipLineNo = Convert.ToInt64(dr["関連伝票行NO"]),
                            Jancode = dr["JANCODE"].ToString() ?? string.Empty,
                            CostFlg = Convert.ToInt32(dr["原価FLG"]),
                            CompleteFlg = Convert.ToInt32(dr["完了FLG"]),
                            DeliverDate = dr["納品日"].ToString() ?? string.Empty,
                            ApproveFlg = int.TryParse(dr["明細承認FLG"].ToString(), out var approve_flg) ? approve_flg : 0,
                            ColorName = dr["COL名"].ToString() ?? string.Empty,
                            SizeName = dr["サイズ名"].ToString() ?? string.Empty,
                            Retail = Convert.ToDecimal(dr["上代"]),
                            WholesalesRate = dr["下代掛率"].ToString() ?? string.Empty,
                            DM1 = dr["DM1"].ToString() ?? string.Empty,
                            SwatchPage = dr["スワッチ頁"].ToString() ?? string.Empty,
                            SwatchPosition = dr["スワッチ位置"].ToString() ?? string.Empty,
                            DeliverDestName = dr["納品先名"].ToString() ?? string.Empty,
                            StaffName = dr["担当者名"].ToString() ?? string.Empty
                        }).OrderBy(x => x.SeqNo);
            Inp12DetailItems = new ObservableCollection<Inp12DetailItem>(list);

            /* 完了FLG追加 2007.11.27 */
            foreach (var row in Inp12DetailItems)
            {
                int comp_flg = int.TryParse(row.CompleteFlg.ToString(), out var flg) ? flg : 0;
                row.WholesalesRate = StatusList[comp_flg];

                /* 計算掛率を算出 */
                OnCalcKakeRitu(row);

                /* 納品日を表示用納品日にセット */
                row.DM1 = row.DeliverDate;

                /* 明細承認FLGを表示用明細承認FLGにセット */
                row.DeliverDestName = row.ApproveFlg.ToString(); 
            }

            int? kei = 0; /* 数量 */
            int? koukei = 0; /* 上代金額 */
            int? koukei1 = 0;/* 下代金額 */

            kei = list.Sum(x => x.Num);
            koukei = list.Sum(x => x.RetailAmount);
            koukei1 = list.Sum(x => x.WholesalesAmount);

            Inp12DetailOpt.Num = kei;
            Inp12DetailOpt.RetailPrice = koukei;
            Inp12DetailOpt.WholesalePrice = koukei1;
            Mess2 = "明細データ取得しました";
            Chg_flg = 1;
        }
        [RelayCommand]
        private void DoDelete()
        {
            if (SelOrderItem == null) return;
            DateTime invDate = DateTime.ParseExact(SelOrderItem.InvCountDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            if (AppData.ClassCvnet.CheckImpDate(invDate) < 0)
            {
                Mess2 = "修正可能な伝票ではありません。";
                return;
            }
            /* 完了確認 */
            /* 2018.10.03 参照箇所調整(明細の完了FLG) */
            if (SelOrderItem.CompleteFlg == 1)
            {
                Mess2 = "出荷指示済の行が存在する為、削除できません。";
                return;
            }
            /* 法人連携 10.10.05 */
            var messbox = System.Windows.MessageBox.Show("削除しますか?", "確認" , System.Windows.MessageBoxButton.YesNo);
            if (messbox == System.Windows.MessageBoxResult.Yes)
            {
                BizArray col_wrk = new BizArray();
                BizArray col_wrk2 = new BizArray();
                col_wrk[0] = "MOD_SEQ";
                col_wrk2[0] = "0";
                var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "Tran_TORI0", 
                                                       long.Parse(SelOrderItem.SeqNo.ToString()), 
                                                       SelOrderItem.VdateUpdate.ToString(), 
                                                       col_wrk.ToArray(), col_wrk2.ToArray());
                if (ret.Code == 0)
                {
                    Mess2 = "データ削除しました";
                    OrderList.Remove(SelOrderItem);
                }
                else if (ret.Code == -1)
                    Mess2 = "ロックエラーです";
                else if (ret.Code == -2)
                    Mess2 = "他で更新されていますので、登録されていません";
                else
                    Mess2 = "データ削除できませんでした";
            }
        }

        [RelayCommand]
        public async Task DoPrint()
        {
            CSV_flg = 0;

            /* パラメータ生成 */
            CreatePara();

            Mess2 = "スプール中です";
            var ret_csv = "";
            if (Inp12SearchOpt.SelPrintCond == 0) 
                /* 一覧 */
                ret_csv = OnQueryPrint(param1, param2); 
            else 
                /* 明細 */
                ret_csv = OnQueryDetailPrint(param1, 1, param2);

            var lines = ret_csv.Split('\n');

            if (lines.Length < 2 || lines[1] == "0")
            {
                ClientLib.MessageBoxError(this, "PDFデータがありません");
                return;
            }

            string pdfPath = lines[0];
            string url = AppData.Http!.URLroot + pdfPath + "/data.pdf";

            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "PDF生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }
            var win = new WebpdfView();
            if (win.DataContext is WebpdfViewModel vm)
            {
                vm.Pdfdata = url;
            }
            ClientLib.CursorToNormal();
            ClientLib.ShowDialogView(win, this);
            Mess2 = "印刷しました";
        }

        [RelayCommand]
        public void DoCSV()
        { 
        
        }
        #endregion

        #region Function
        /* パラメータ生成 */
        private void CreatePara()
        {
            if (Inp12SearchOpt == null) return;
            param1 = new BizArray();
            param2 = new BizArray();

            param1[0] = Inp12SearchOpt.SlipStartNo;
            param1[1] = Inp12SearchOpt.SlipEndNo;
            param1[2] = Inp12SearchOpt.OrderStartDate?.ToString("yyyyMMdd");
            param1[3] = Inp12SearchOpt.OrderEndDate?.ToString("yyyyMMdd");
            param1[4] = Inp12SearchOpt.TranStartCat;
            param1[5] = Inp12SearchOpt.TranEndCat;
            param1[6] = Inp12SearchOpt.RelatedStartNo;
            param1[7] = Inp12SearchOpt.RelatedEndNo;
            param1[8] = Inp12SearchOpt.RelatedStartNo2;
            param1[9] = Inp12SearchOpt.RelatedEndNo2;
            param1[10] = Inp12SearchOpt.CustStartDest.Code;
            param1[11] = Inp12SearchOpt.CustEndDest.Code;
            param1[12] = Inp12SearchOpt.StoreStartCd.Code;
            param1[13] = Inp12SearchOpt.StoreEndCd.Code;
            param1[14] = Inp12SearchOpt.ExhibitStartCd.Code;
            param1[15] = Inp12SearchOpt.ExhibitEndCd.Code;
            param1[16] = Inp12SearchOpt.ManualInpStartNo;
            param1[17] = Inp12SearchOpt.ManualInpEndNo;
            param1[18] = Inp12SearchOpt.InpStartCd.Code;
            param1[19] = Inp12SearchOpt.InpEndCd.Code;
            if (PrdToggle == 1)
            {
                param2[0] = Inp12SearchOpt.ProdStartCd.Code;
                param2[1] = Inp12SearchOpt.ProdEndCd.Code;
            }
        }

        /* ヘッダ検索処理 */
        private void OnQuery(string[] v_para1, string[] v_sho, string wrk_para = "", string p_sort = "")
        {
            /* 戻るボタン対応  20080702 */
            var v_sort = "DESC";
            var v_hugo = "<=";
            if (p_sort != "")
            {
                v_sort = p_sort;
                v_hugo = ">=";
            }

            DataTable ret_csv = new DataTable();

            var v_para = new BizArray();
            for (int i = 0; i < v_para1.Length; i++)
            {
                v_para[i] = v_para1[i];
            }

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            for (var i = 0; i < col_list.Count; i++)
            {
                sql_query += "A." + col_list[i] + ",";
            }
            sql_query += "B.名前 入力社員名,C.得意先名 得意先名,";
            sql_query += "NVL((SELECT 得意先名 FROM HC$MASTER_TOKUI WHERE 得意先CD = A.倉庫CD),'') 倉庫名,";
            sql_query += "C.掛率,C.セール掛率,C.消費税CD,C.消費税計算方法,C.消費税端数,C.下代桁切指定,C.下代端数区分,C.下代計算FLG";

            sql_query += ",NVL((SELECT MAX(T1.完了FLG) 完了FLG FROM HC$TRAN_TORI1 T1 WHERE A.SEQ_NO=T1.ヘッダNO GROUP BY T1.ヘッダNO),0) 完了FLG";
            sql_query += ",NVL((SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分='TNJ' AND 名称CD = A.展示会CD),'') 展示会名";
            sql_query += ",NVL((SELECT n.納品先名 FROM HC$MASTER_NOHIN n WHERE n.納品先CD = A.納品先CD),'') 納品先名";
            sql_query += ",NVL((SELECT n.名前 FROM HC$MASTER_SHAIN n WHERE n.社員CD = A.担当者CD),'') 担当者名";

            sql_query += " from HC$Tran_TORI0 A,HC$MASTER_SHAIN B,HC$Master_TOKUI C ";
            sql_query += "where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A.在庫計上日 between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.倉庫CD between :13 and :14 ";
            sql_query += "and A.展示会CD between :15 and :16 ";
            sql_query += "and A.手入力伝票NO between :17 and :18 ";
            sql_query += "and A.入力社員CD between :19 and :20 ";
            sql_query += " and A.伝票処理区分=12";

            /* 20210205 ページ表示機能 */
            if (!string.IsNullOrEmpty(wrk_para))
            {
                v_para.Add(wrk_para);
                sql_query += " AND A.SEQ_NO " + v_hugo + " :" + v_para.Count;
            }

            if (PrdToggle == 1 && (v_sho != null && v_sho.Length > 1)) 
            {
                v_para.Add(v_sho[0]);
                v_para.Add(v_sho[1]);
                sql_query = "SELECT A.* FROM (" + sql_query
                + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (v_para.Count - 1).ToString() + " AND :" + (v_para.Count).ToString() + ")";
            }
            sql_query += " ORDER BY A.SEQ_NO " + v_sort; 
            sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query); 
            ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
             
            var list = (from DataRow dr in ret_csv.Rows
                        select new SelOrderTran
                        {
                            SeqNo = Convert.ToInt32(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDouble(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDouble(dr["VDATE_UPDATE"]),
                            RelatedSlipNo = Convert.ToInt64(dr["関連伝票NO"]),
                            InvCountDate = dr["在庫計上日"].ToString() ?? string.Empty,
                            DeliverDate = dr["納品日"].ToString() ?? string.Empty,
                            TranCate = dr["取引区分"].ToString() ?? string.Empty,
                            InpStaffCD = dr["入力社員CD"].ToString() ?? string.Empty,
                            StoreCD = dr["倉庫CD"].ToString() ?? string.Empty,
                            ClientCD1 = dr["取引先CD1"].ToString() ?? string.Empty,
                            MarkupRate1 = Convert.ToSingle(dr["掛率1"]),
                            TaxableAmount = Convert.ToInt64(dr["外税対象金額"]),
                            TotalNum = Convert.ToDouble(dr["数量合計"]),
                            TotalDetailAmt = Convert.ToInt64(dr["明細金額合計"]),
                            TaxIncluded = Convert.ToInt64(dr["内税消費税"]),
                            TaxExcluded = Convert.ToInt64(dr["外税消費税"]),
                            TotalRetail = Convert.ToInt64(dr["上代合計"]),
                            TotalWholesale = Convert.ToInt64(dr["下代合計"]),
                            Memo = dr["メモ"].ToString() ?? string.Empty,
                            MarkupRate2 = Convert.ToSingle(dr["掛率2"]),
                            SlipCate = Convert.ToInt16(dr["伝票処理区分"]),
                            ModSeq = Convert.ToInt64(dr["MOD_SEQ"]),
                            RelatedSlipNo2 = Convert.ToInt64(dr["関連伝票NO2"]),
                            ExhibitCD = dr["展示会CD"].ToString() ?? string.Empty,
                            ManualInpNo = dr["手入力伝票NO"].ToString() ?? string.Empty,
                            ShipToCD = dr["納品先CD"].ToString() ?? string.Empty,
                            StaffCD = dr["担当者CD"].ToString() ?? string.Empty,
                            InpStaffName = dr["入力社員名"].ToString() ?? string.Empty,
                            TradeName = dr["得意先名"].ToString() ?? string.Empty,
                            StoreName = dr["倉庫名"].ToString() ?? string.Empty,
                            MarkupRate = float.TryParse(dr["掛率"].ToString(), out var mark) ? mark : 0,
                            SalesRate = float.TryParse(dr["セール掛率"].ToString(), out var sales) ? sales : 0,
                            TaxCD = long.TryParse(dr["消費税CD"].ToString(), out var tax_cd) ? tax_cd : 0,
                            TaxCalcMethod = short.TryParse(dr["消費税計算方法"].ToString(), out var tax_method) ? tax_method : (short)0,
                            TaxRound = short.TryParse(dr["消費税端数"].ToString(), out var tax_round) ? tax_round : (short)0,
                            CostDigitCut = short.TryParse(dr["下代桁切指定"].ToString(), out var cost_digit) ? cost_digit : (short)0,
                            CostRoundType = short.TryParse(dr["下代端数区分"].ToString(), out var cost_round) ? cost_round : (short)0,
                            CostCalcFlg = short.TryParse(dr["下代計算FLG"].ToString(), out var cost_flg) ? cost_flg : (short)0,
                            CompleteFlg = short.TryParse(dr["完了FLG"].ToString(), out var compl_flg) ? compl_flg : (short)0,
                            ExhibitName = dr["展示会名"].ToString() ?? string.Empty,
                            ShipToName = dr["納品先名"].ToString() ?? string.Empty,
                            StaffName = dr["担当者名"].ToString() ?? string.Empty,
                        }).OrderByDescending(c => c.SeqNo).ToList(); 
            OrderList = new ObservableCollection<SelOrderTran>(list);  
        }

        /*ヘッダ印刷BTN用検索処理 */
        private string OnQueryPrint(BizArray wrk_para1, BizArray v_sho)
        {
            var wrk_para = new BizArray();
            for (var i = 0; i < wrk_para1.Count; i++)
            {
                wrk_para[i] = wrk_para1[i];
            }

            /* 05.03.20 F-T変更 */
            var sql_query = "select A.SEQ_NO"; /* 伝票NO */
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時"; /* データを日付として表示させるための処理 */
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時,";
            sql_query += "'受注伝票一覧'"; /* Pssタイトル用 */

            for (var i = 0; i < col_list.Count; i++)
            {
                sql_query += ",A." + col_list[i];
            }

            sql_query += ", A.SYSFLG, A.送信FLG";
            sql_query += ",B.名前 担当名,C.得意先名 得意先名";
            sql_query += ",(SELECT 得意先名 FROM HC$MASTER_TOKUI WHERE 得意先CD = A.倉庫CD AND (店種区分=0 OR 倉庫区分=9)) 倉庫名";
            sql_query += ",C.掛率,C.セール掛率,C.消費税CD,C.消費税計算方法,C.消費税端数,C.下代桁切指定,C.下代端数区分,C.下代計算FLG";

            /* 画面上ComboBoxの文字列取得 */
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("商品受注区分", "A.取引区分") + " 取引区分名";        /* 関数Combo処理 */
            sql_query += ",(SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分 = 'TNJ' AND 名称CD = A.展示会CD) 展示会";
            sql_query += ",NVL((SELECT n.納品先名 FROM HC$MASTER_NOHIN n WHERE n.納品先CD = A.納品先CD),'') 納品先名";
            sql_query += ",NVL((SELECT n.名前 FROM HC$MASTER_SHAIN n WHERE n.社員CD = A.担当者CD),'') 担当者名";

            sql_query += " from HC$Tran_TORI0 A,HC$MASTER_SHAIN B,HC$Master_TOKUI C ";
            sql_query += "where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A.在庫計上日 between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.倉庫CD between :13 and :14 ";
            sql_query += "and A.展示会CD between :15 and :16 ";
            sql_query += "and A.手入力伝票NO between :17 and :18 ";
            sql_query += "and A.入力社員CD between :19 and :20 ";
            sql_query += "and A.伝票処理区分=12";

            if (PrdToggle == 1)
            {
                wrk_para[wrk_para.Count] = v_sho[0];
                wrk_para[wrk_para.Count] = v_sho[1];
                sql_query = "SELECT A.* FROM (" + sql_query
                + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (wrk_para.Count - 1) + " AND :" + (wrk_para.Count) + ")";
            }
            sql_query += " order by A.SEQ_NO";

            /* 2016.09.17 CSV出力用(#30512) */
            if (CSV_flg == 1)
            {
                sql_query = ""
                  + "select "
                  + " a.SEQ_NO 伝票No , "
                  + " a.在庫計上日 受注日 , "
                  + " a.納品日 , "
                  + " a.伝票処理区分 伝票区分 , "
                  + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ')-1) 取引区分CD , "
                  + " substrb(a.取引区分名, instr(a.取引区分名, ' ')+1) 取引区分名 , "
                  + " a.掛率1 掛率 , "
                  + " a.SYSFLG , "
                  + " a.送信FLG , "
                  + " a.数量合計 数量計 , "
                  + " a.明細金額合計 金額計 , "
                  + " a.上代合計 , "
                  + " a.取引先CD1 得意先CD , "
                  + " a.得意先名 , "
                  + " a.倉庫CD , "
                  + " a.倉庫名 , "
                  + " a.入力社員CD 入力者CD , "
                  + " a.担当名 入力者名 , "
                  + " a.展示会CD , "
                  + " a.展示会 展示会名 , "
                  + " a.外税消費税 消費税 , "
                  + " a.下代合計 , "
                  + " a.手入力伝票NO 手入力No , "
                  + " a.関連伝票NO 関連No1 , "
                  + " a.関連伝票NO2 関連No2 , "
                  + " a.納品先CD , "
                  + " a.納品先名 , "
                  + " a.担当者CD 営業担当CD , "
                  + " a.担当者名 営業担当名 , "
                  + " a.メモ "
                  + " from "
                  + "(" + sql_query + ") a";
            }
            return AppData.Http!.AspxSqlQueryCsv(sql_query, wrk_para.ToArray(), "cvnet12prn_header.qfm");
        }
        /*明細印刷BTN用検索処理 */
        private string OnQueryDetailPrint(BizArray wrk_para1, int hdFlg, BizArray v_sho)
        {
            var wrk_para = new BizArray();
            for (var i = 0; i < wrk_para1.Count; i++)
            {
                wrk_para[i] = wrk_para1[i];
            }
            var sql_query = "select A.SEQ_NO";      /* 伝票NO */
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";         /* データを日付として表示させるための処理 */
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時";
            sql_query += ",'受注伝票明細'";		/* Pssタイトル用 */
            for (var i = 0; i < col_list.Count; i++)
            {   
                /* 伝票SQL */
                sql_query += ",A." + col_list[i];
            }
            sql_query += ",A.SYSFLG, A.送信FLG";
            sql_query += ",B.名前 担当名,C.得意先名 得意先名";
            sql_query += ",(SELECT 得意先名 FROM HC$MASTER_TOKUI WHERE 得意先CD = A.倉庫CD AND (店種区分=0 OR 倉庫区分=9)) 倉庫名";
            sql_query += ",C.掛率,C.セール掛率,C.消費税CD,C.消費税計算方法,C.消費税端数,C.下代桁切指定,C.下代端数区分,C.下代計算FLG";
            for (var i = 0; i < col_list2.Count; i++)
            {   
                /* 明細SQL */
                sql_query += ",T1." + col_list2[i] + " 明細_" + col_list2[i].ToString();
            }
            sql_query += ",NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=T1.色CD),'.') COL名";
            sql_query += ",GET_SIZENAME(T1.商品CD, T1.サイズCD) サイズ名, get_jodai(T1.商品CD, T1.色CD, T1.サイズCD) 上代";

            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("商品受注区分", "A.取引区分") + " 取引区分名";
            sql_query += ",T1.行NO";
            sql_query += ",decode(T1.完了FLG,1,'完了','未完') 完了FLG名";    /* 完了FLG追加 2007.11.27 */
            sql_query += ",(SELECT 名称 FROM HC$MASTER_MEISHO WHERE 名称区分 = 'TNJ' AND 名称CD = A.展示会CD) 展示会";

            /* 10.01.08 追加*/
            sql_query += " ,NVL((SELECT MAX(TO_CHAR(W.PAGE)) FROM HC$TRAN_TENSWT W WHERE W.展示会CD=A.展示会CD AND W.商品CD=T1.商品CD),'.') スワッチ頁";
            sql_query += " ,NVL((SELECT MAX(TO_CHAR(W.POS)) FROM HC$TRAN_TENSWT W WHERE W.展示会CD=A.展示会CD AND W.商品CD=T1.商品CD),'.') スワッチ位置";

            /* 2016.09.21 追加 */
            sql_query += ",NVL((SELECT n.納品先名 FROM HC$MASTER_NOHIN n WHERE n.納品先CD = A.納品先CD),'') 納品先名";
            sql_query += ",NVL((SELECT n.名前 FROM HC$MASTER_SHAIN n WHERE n.社員CD = A.担当者CD),'') 担当者名";

            sql_query += " from HC$Tran_TORI0 A, HC$MASTER_SHAIN B, HC$Master_TOKUI C ";
            sql_query += " ,HC$tran_tori1 T1";

            sql_query += " where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.得意先CD(+)) ";
            sql_query += " and A.SEQ_NO = T1.ヘッダNO";
            sql_query += " and A.伝票処理区分=12";

            if (hdFlg == 1)
            {
                /* 検索画面「明細印刷」 */
                sql_query += "and A.SEQ_NO between :1 and :2 and A.在庫計上日 between :3 and :4 and A.取引区分 between :5 and :6 ";
                sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.倉庫CD between :13 and :14 ";
                sql_query += "and A.展示会CD between :15 and :16 ";
                sql_query += "and A.手入力伝票NO between :17 and :18 ";
                sql_query += "and A.入力社員CD between :19 and :20 ";
                if (PrdToggle == 1)
                {
                    wrk_para.Add(v_sho[0]);
                    wrk_para.Add(v_sho[1]);
                    sql_query += " AND EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$TRAN_TORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (wrk_para.Count - 1) + " AND :" + (wrk_para.Count) + ")";
                }
            }
            else
            {
                /* 修正・登録画面「印刷」 */
                sql_query += " and T1.ヘッダNO=:1 and T1.明細取引区分=:2";
            }
            sql_query += " order by T1.ヘッダNO, T1.行NO";

            /* 2016.09.17 CSV出力用(#30512) */
            if (CSV_flg == 1)
            {
                sql_query = ""
                + "select "
                    + " a.SEQ_NO 伝票No , "
                    + " a.在庫計上日 受注日 , "
                    + " a.納品日 , "
                    + " a.伝票処理区分 伝票区分 , "
                    + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ')-1) 取引区分CD , "
                    + " substrb(a.取引区分名, instr(a.取引区分名, ' ')+1) 取引区分名 , "
                    + " a.掛率1 掛率 , "
                    + " a.SYSFLG , "
                    + " a.送信FLG , "
                    + " a.数量合計 数量計 , "
                    + " a.明細金額合計 金額計 , "
                    + " a.上代合計 , "
                    + " a.下代合計 , "
                    + " a.取引先CD1 得意先CD , "
                    + " a.得意先名 , "
                    + " a.倉庫CD , "
                    + " a.倉庫名 , "
                    + " a.入力社員CD 入力者CD , "
                    + " a.担当名 入力者名 , "
                    + " a.展示会CD , "
                    + " a.展示会 展示会名 , "
                    + " a.外税消費税 消費税計 , "
                    + " a.手入力伝票NO 手入力No ,"
                    + " a.関連伝票NO 関連No1 , "
                    + " a.関連伝票NO2 関連No2 , "
                    + " a.納品先CD , "
                    + " a.納品先名 , "
                    + " a.担当者CD 営業担当CD , "
                    + " a.担当者名 営業担当名 , "
                    + " a.メモ , "
                    + " a.行NO 行No , "
                    + " a.明細_商品CD 商品CD , "
                    + " a.明細_明細名称 商品名 , "
                    + " a.明細_色CD 色CD , "
                    + " a.COL名 色名 , "
                    + " a.明細_サイズCD サイズCD , "
                    + " a.サイズ名 , "
                    + " a.明細_数量 数量 , "
                    + " a.明細_単価 単価 , "
                    + " a.明細_上代単価 上代単価 , "
                    + " a.明細_下代単価 下代単価 , "
                    + " a.明細_原価FLG 原価FLG , "
                    + " a.明細_納品日 明細納品日 , "
                    + " a.明細_明細メモ 摘要 ,"
                    + " a.明細_完了FLG 完了FLG , "
                    + " a.完了FLG名 完了 , "
                    + " a.スワッチ頁 , "
                    + " a.スワッチ位置 , "
                    + " a.明細_外税消費税 消費税 , "
                    + " a.明細_金額 金額 , "
                    + " a.明細_上代金額 上代金額 , "
                    + " a.明細_下代金額 下代金額 , "
                    /* 2021.04.27 追加 */
                    + " nvl(s.上代,0) マスタ上代 , "
                    + " nvl(s.元上代,0) 元上代 , "
                    + " nvl(s.仕入価格,0) 仕入価格 , "
                    + " GET_GENKA(a.明細_商品CD,0,a.在庫計上日) マスタ原価 , "
                    + " nvl(s.営業原価,0) 営業原価 , "
                    + " nvl(s.旧コード,'') 旧コード , "
                    + " nvl(s.メーカー品番,'') メーカー品番 , "
                    + " nvl(s.略称,'') 略称 , "
                    + " nvl(s.仕入区分,0) 仕入区分 , "
                    + " decode(nvl(s.仕入区分,0),1,'買取',2,'委託',3,'消化','') 仕入区分名 , "
                    + " nvl(s.展示会CD,'') 展示会CD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='TNJ' and m.名称CD=s.展示会CD),'') 展示会名 , "
                    + " nvl(s.ブランドCD,'') ブランドCD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='BRD' and m.名称CD=s.ブランドCD),'') ブランド名 , "
                    + " nvl(s.アイテムCD,'') アイテムCD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='ITM' and m.名称CD=s.アイテムCD),'') アイテム名 , "
                    + " nvl(s.シーズンCD,'') シーズンCD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='SZN' and m.名称CD=s.シーズンCD),'') シーズン名 , "
                    + " nvl(s.素材CD,'') 素材CD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='SZI' and m.名称CD=s.素材CD),'') 素材名 , "
                    + " nvl(s.デザイナーCD,'') デザイナーCD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='DZN' and m.名称CD=s.デザイナーCD),'') デザイナー名 , "
                    + " nvl(s.メーカーCD,'') メーカーCD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='MKR' and m.名称CD=s.メーカーCD),'') メーカー名 , "
                    + " nvl(s.原産国CD,'') 原産国CD , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='GEN' and m.名称CD=s.原産国CD),'') 原産国名 , "
                    + " nvl(s.名称CD01,'') 名称CD01 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B01' and m.名称CD=s.名称CD01),'') 名称CD01名 , "
                    + " nvl(s.名称CD02,'') 名称CD02 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B02' and m.名称CD=s.名称CD02),'') 名称CD02名 , "
                    + " nvl(s.名称CD03,'') 名称CD03 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B03' and m.名称CD=s.名称CD03),'') 名称CD03名 , "
                    + " nvl(s.名称CD04,'') 名称CD04 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B04' and m.名称CD=s.名称CD04),'') 名称CD04名 , "
                    + " nvl(s.名称CD05,'') 名称CD05 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B05' and m.名称CD=s.名称CD05),'') 名称CD05名 , "
                    + " nvl(s.名称CD06,'') 名称CD06 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B06' and m.名称CD=s.名称CD06),'') 名称CD06名 , "
                    + " nvl(s.名称CD07,'') 名称CD07 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B07' and m.名称CD=s.名称CD07),'') 名称CD07名 , "
                    + " nvl(s.名称CD08,'') 名称CD08 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B08' and m.名称CD=s.名称CD08),'') 名称CD08名 , "
                    + " nvl(s.名称CD09,'') 名称CD09 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B09' and m.名称CD=s.名称CD09),'') 名称CD09名 , "
                    + " nvl(s.名称CD10,'') 名称CD10 , "
                    + " nvl((select m.名称 from HC$MASTER_MEISHO m where m.名称区分='B10' and m.名称CD=s.名称CD10),'') 名称CD10名 , "
                    + " nvl(j.JANコード1,'') JANコード1 , "
                    + " nvl(j.JANコード2,'') JANコード2 , "
                    + " nvl(j.JANコード3,'') JANコード3 "
                + "  from "
                    + "(" + sql_query + ") a , "
                    + " HC$MASTER_SHOHIN s , "
                    + " HC$MASTER_SHOHIN_JAN j "
                + " where "
                    + " a.明細_商品CD=s.商品CD(+) "
                    + " and a.明細_商品CD=j.商品CD(+) "
                    + " and a.明細_色CD=j.色CD(+) "
                    + " and a.明細_サイズCD=j.サイズCD(+) "
                + " order by "
                    + " a.SEQ_NO , "
                    + " a.行NO "
                + "";
            }
            return AppData.Http!.AspxSqlQueryCsv(sql_query, wrk_para.ToArray(), "cvnet12prn_detail.qfm");
        }
        /* 明細検索処理 */
        private DataTable OnQueryDetail(BizArray v_para)
        {
            /* 展示会スワッチを参照 10.01.08 */
            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            for (var i = 0; i < col_list2.Count; i++)
            {
                sql_query += "A." + col_list2[i] + ",";
            }
            sql_query += " NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') COL名";
            sql_query += ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";
            sql_query += ", get_jodai(A.商品CD,A.色CD,A.サイズCD,:0,:1) 上代";
            sql_query += " ,get_kakeritu('" + Inp12DetailOpt.CustDest + "',10,A.商品CD) 下代掛率";

            sql_query += " ,'' DM1,NVL((SELECT MAX(TO_CHAR(W.PAGE)) FROM HC$TRAN_TENSWT W,HC$TRAN_TORI0 T0 WHERE A.ヘッダNO=T0.SEQ_NO AND W.展示会CD=T0.展示会CD AND W.商品CD=A.商品CD),'.') スワッチ頁";
            sql_query += " ,NVL((SELECT MAX(TO_CHAR(W.POS)) FROM HC$TRAN_TENSWT W,HC$TRAN_TORI0 T0 WHERE A.ヘッダNO=T0.SEQ_NO AND W.展示会CD=T0.展示会CD AND W.商品CD=A.商品CD),'.') スワッチ位置";

            /* 2016.09.21 追加 */
            sql_query += ",NVL((SELECT n.納品先名 FROM HC$MASTER_NOHIN n WHERE n.納品先CD = T0.納品先CD),'') 納品先名";
            sql_query += ",NVL((SELECT n.名前 FROM HC$MASTER_SHAIN n WHERE n.社員CD = T0.担当者CD),'') 担当者名";

            sql_query += " from HC$tran_tori1 A, HC$tran_tori0 T0";
            sql_query += " where A.ヘッダNO=:2 and A.明細取引区分=:3 and T0.SEQ_NO = A.ヘッダNO";
            sql_query += " order by A.ヘッダNO, A.行NO";
             
            var ret_data = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray()); 
            return ret_data;
        }
        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Variables
        [ObservableProperty]
        public Inp12DetailOpt m_Inp12DetailOpt;

        [ObservableProperty]
        public ObservableCollection<Inp12DetailItem> m_Inp12DetailItems;
        #endregion

        #region Text Variable
        private int Text20 = 12;
        /* ｾｰﾙ掛率区分 */
        private int Text19;
        /* 得意先、掛率 */
        private int Text30;
        /* 得意先、セール掛率 */
        private int Text31;
        /* 得意先、消費税CD */
        private int Text32;
        /* 得意先、消費税計算方法 */
        private int Text33;
        /* 得意先、消費税端数 */
        private int Text34;
        /* 得意先、下代桁切指定 */
        private int Text35;
        /* 得意先、下代端数区分 */
        private int Text36;
        /* 得意先、下代計算FLG */
        private int Text37;
        #endregion

        #region ComboList Variable
        [ObservableProperty]
        public Dictionary<int, string> m_StatusList;
        #endregion

        #region Dialog Search
        [RelayCommand]
        public void SelCustDest(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12DetailOpt != null)
            {
                Inp12DetailOpt.CustDest = new BtListHelper(get_sel00.Code, get_sel00.Name); 
            }
        }
        [RelayCommand]
        public void SelStoreCd(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12DetailOpt != null)
            {
                Inp12DetailOpt.StoreCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelInpCd(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12DetailOpt != null)
            {
                Inp12DetailOpt.InpCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelExhibitCd(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12DetailOpt != null)
            {
                Inp12DetailOpt.ExhibitCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelSalesRep(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12DetailOpt != null)
            {
                Inp12DetailOpt.SalesRep = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        public void SelProdCd(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp12DetailOpt != null)
            {
                Inp12DetailOpt.ProdCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        #endregion

        #region Events
        [RelayCommand]
        public void DoBatchSet()
        {
            if (Inp12DetailItems.Count == 0) return;
            if (Inp12DetailOpt == null || string.IsNullOrEmpty(Inp12DetailOpt.CustDest.Code)) return;
            var messbox = System.Windows.MessageBox.Show("全明細に納品日を反映させます", "確認", System.Windows.MessageBoxButton.OKCancel);
            if (messbox == System.Windows.MessageBoxResult.OK)
            {
                foreach (var item in Inp12DetailItems)
                {
                    item.DeliverDate = Inp12DetailOpt.DeliverDate?.ToString("yyyyMMdd");
                }
            }
        }
        [RelayCommand]
        public void DoExpandRate()
        {
            if (Inp12DetailItems.Count == 0) return;
            if (Inp12DetailOpt == null || string.IsNullOrEmpty(Inp12DetailOpt.CustDest.Code)) return;
            var messbox = System.Windows.MessageBox.Show("全明細に掛率を反映させます", "確認", System.Windows.MessageBoxButton.OKCancel);
            if (messbox == System.Windows.MessageBoxResult.OK)
            {
                var ritu = Inp12DetailOpt.MarkupRate;
                foreach (var item in Inp12DetailItems)
                {
                    if (ritu < 0) {
                        ritu = item.BaseRate;
                    }
                    int _ritu = int.TryParse(ritu.ToString(), out int _r) ? _r : 0;
                    item.WholesalesUnit = int.TryParse(OnGetGedai(item.RetailUnitPrice, _ritu).ToString(), out int _whole) ? _whole : 0;
                    item.UnitPrice = item.WholesalesUnit;
                    item.WholesalesAmount = item.Num * item.WholesalesUnit;
                    item.TotalAmount = item.Num * item.UnitPrice;

                    /* 計算に使用した率を計算掛率に表示 */
                    SetKakeRitu(item, _ritu);
                    OnChangeTotal(item);
                }
            }
        }

        [RelayCommand]
        public void DoAllClear()
        {
            if (Inp12DetailItems.Count == 0) return;
            var messbox = System.Windows.MessageBox.Show("明細表示が全てクリアされます。よろしいですか？", "確認", System.Windows.MessageBoxButton.OKCancel);
            if (messbox == System.Windows.MessageBoxResult.OK)
            {
                Inp12DetailOpt = new Inp12DetailOpt();
                Inp12DetailOpt.OrderDate = DateTime.Now;
                Inp12DetailOpt.DeliverDate = DateTime.Now;

                if (AppData.ClassCvnet.SysImp != null && AppData.ClassCvnet.SysImp.Rows.Count > 0)
                    Inp12DetailOpt.StoreCd = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(), AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
                else
                    Inp12DetailOpt.StoreCd = new BtListHelper("", "");
                Inp12DetailOpt.StoreCd.pre_data = new BtListHelper("", "");
                Inp12DetailOpt.CustDest = new BtListHelper("", "");
                Inp12DetailOpt.CustDest.pre_data = new BtListHelper("", "");

                Inp12DetailOpt.InpCd = new BtListHelper(AppData.ClassSatoo.SHAIN_CD, AppData.ClassSatoo.SHAIN_Name);
                Text20 = 12; 
                Text19 = 0; 
                Text30 = 0; 
                Text31 = 0;
                Text32 = 0;
                Text33 = 0;
                Text34 = 0;
                Text35 = 0;
                Text36 = 0;
                Text37 = 0;
                Inp12DetailOpt.TranCate = "10";
            }
        }

        [RelayCommand]
        public void DoAvailProd()
        {
            if (Inp12DetailOpt == null || Inp12DetailOpt.ProdCd == null) return;
            if (string.IsNullOrEmpty(Inp12DetailOpt.CustDest.Code))
            {
                Mess2 = "先に得意先を入力してください";
                return;
            }
            if (OnCheckMst() < 0) return;

            var v_para = new BizArray();
            v_para[0] = Inp12DetailOpt.ProdCd.Code;
            /* 下代掛率計算用得意先と取引区分を追加 */
            v_para[1] = Inp12DetailOpt.CustDest.Code;
            v_para[2] = "10";
            v_para[3] = Text35.ToString(); /* 下代桁 */
            v_para[4] = Text36.ToString(); /* 下代端数 */

            var v_para2 = new BizArray();
            v_para2[0] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
            v_para2[1] = Inp12DetailOpt.StoreCd.Code;
            v_para2[2] = Text20.ToString();
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) v_para2[0] = "19000101";
             
            var vm_result = AppData.DlgService.GetSku01(v_para.ToArray(), v_para2.ToArray());
            if (vm_result != null && vm_result.ret_para != null) 
            {
                OnSetMultiRow(vm_result.ret_para, "2");
            }
        }

        [RelayCommand]
        public void InpBarcode()
        {
            if (string.IsNullOrEmpty(Inp12DetailOpt.CustDest.Code))
            {
                Mess2 = "先に得意先を入力してください";
                return;
            }
            /* マスタチェック */
            if (OnCheckMst() < 0) return;

            var wrk_para = new BizArray();
            wrk_para[0] = Text20.ToString();
            wrk_para[1] = Inp12DetailOpt.CustDest.Code;
            wrk_para[2] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
            /* ジャコモ用取引区分追加 */
            wrk_para[5] = Inp12DetailOpt.TranCate;

            var vm_result = AppData.DlgService.GetBcd01(wrk_para.ToArray());
            if (vm_result != null && vm_result.ret_para != null)
            {
                OnSetMultiRow(vm_result.ret_para, "3");
            }
        }

        [RelayCommand]
        public void DoProdRowSearch(Inp12DetailItem value)
        {
            var ar2 = new BizArray();
            /* セールマスタ参照用 色、サイズ、在庫計上日、店舗 */
            if (string.IsNullOrEmpty(value.ColorCD)) 
                ar2[0] = ".";
            else ar2[0] = value.ColorCD;
            if (string.IsNullOrEmpty(value.SizeCD))
                ar2[1] = ".";
            else ar2[1] = value.SizeCD;
            ar2[2] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
            ar2[3] = Inp12DetailOpt.StoreCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0)
                ar2[2] = "19000101";

            if (AppData.ClassCvnet.MstDialog.ContainsKey("商品") && AppData.ClassCvnet.ComboListFlg3 == 1)
            {
                var ar = new BizArray();
                ar[0] = "0";
                ar[4] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");

                var wrk_para = new BizArray();
                wrk_para[0] = value.ProdCD;
                wrk_para[3] = value.CostFlg.ToString();
                wrk_para[4] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
                if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para[4] = "19000101";

                var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, wrk_para.ToArray(), ar.ToArray(), null, ar2.ToArray());
                if (vm != null)
                {
                    var data = vm.SelShoResult0; 
                    OnZoomRet(value, vm.SelShoResult0,  1);
                } 
                return;
            }
            else
            {
                //AppData.DlgService.GetSel2
                var wrk_para = new BizArray();
                wrk_para[0] = value.ProdCD;
                wrk_para[3] = value.CostFlg.ToString();
                wrk_para[4] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
                if (AppData.ClassCvnet.config.jodaihyjflg == 0)
                    wrk_para[4] = "19000101";
                AppData.DlgService.GetSel2(wrk_para.ToArray(), null, null, ar2.ToArray());
            }

            value.ColorCD = string.Empty; /* 色 */
            value.SizeCD = string.Empty; /* サイズ */
            value.ColorName = string.Empty; /* 色名 */
            value.SizeName = string.Empty; /* サイズ名 */
        }

        [RelayCommand]
        public void DoColorRowSearch(Inp12DetailItem value)
        {
            var wrk_para = new BizArray();
            wrk_para[0] = value.ProdCD;
            var wrk_para2 = new BizArray();
            wrk_para2[0] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
            wrk_para2[1] = Inp12DetailOpt.StoreCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para2[0] = "19000101"; 
            var vm_result = AppData.DlgService.GetSelclsz0(wrk_para.ToArray(), wrk_para2.ToArray());
            if (vm_result != null)
            { 
                OnZoomRet(value, vm_result.ret_para, 2);
            }
        }

        [RelayCommand]
        public void DoSizeRowSearch(Inp12DetailItem value)
        {
            var wrk_para = new BizArray();
            wrk_para[0] = value.ProdCD;
            wrk_para[1] = value.ColorCD;
            var wrk_para2 = new BizArray();
            wrk_para2[0] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd");
            wrk_para2[1] = Inp12DetailOpt.StoreCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para2[0] = "19000101";
            var vm_result = AppData.DlgService.GetSelclsz0(wrk_para.ToArray(), wrk_para2.ToArray());
            if (vm_result != null)
            {
                OnZoomRet(value, vm_result.ret_para, 3);
            }
        }

        /// <summary>
        /// </summary> 
        /// <param name="mode">1 商品CD, 2 色CD, 3 サイズCD</param>
        private void OnZoomRet(Inp12DetailItem selected, BizArray para, int mode)
        {
            // 2 色CD, 3 サイズCD
            if (mode == 2 || mode == 3)
            {

                /* 色CD,サイズCD,色名,サイズ名,上代 */
                selected.ColorCD = para[0];
                selected.SizeCD = para[1];
                selected.ColorName = para[2];
                selected.SizeName = para[3];
                int retail = int.TryParse(para[4].ToString(), out var price) ? price : 0;
                selected.RetailUnitPrice = retail;
                selected.Retail = retail;

                /* 明細行の計算掛率を使用する */
                var ritu = selected.CalcRate;
                if (ritu < 0)
                    ritu = int.TryParse(Inp12DetailOpt.MarkupRate.ToString(), out var mark) ? mark : 0;

                if (ritu >= 0 && ritu != 100)
                {
                    if (SelOrderItem.CostCalcFlg == 0)
                    {
                        var get_gedai = OnGetGedai(selected.RetailUnitPrice, ritu);
                        selected.WholesalesUnit = int.TryParse(get_gedai.ToString(), out var wholesales) ? wholesales : 0;
                    }
                    else
                    {
                        int retails = int.TryParse(selected.Retail.ToString(), out var _retail) ? _retail : 0;
                        var get_gedai = OnGetGedai(retails, ritu);
                        selected.WholesalesUnit = int.TryParse(get_gedai.ToString(), out var wholesales) ? wholesales : 0;
                    }
                }
                else if (SelOrderItem.CostCalcFlg == 0)
                    selected.WholesalesUnit = selected.RetailUnitPrice;
                else
                {
                    int size = int.TryParse(selected.SizeName, out var _size) ? _size : 0;
                    selected.WholesalesUnit = size;
                }
            }
            // 1 商品CD
            else if (mode == 1)
            {
                selected.ProdCD = para[0];
                selected.DetailName = para[1];
                /* 2012.05.28 USER87は商品名にメーカー品番を表示 */ 
                selected.RetailUnitPrice = int.TryParse(para[2], out int retail_unit) ? retail_unit : 0;
                selected.Retail = selected.RetailUnitPrice;
                selected.DeliverDate = para[16]; /* 納品日追加 2008.09.26 */
                selected.DM1 = para[16]; /* 納品日（表示用）追加 2008.09.26 */
                selected.ApproveFlg = int.TryParse(para[22], out var approve) ? approve : 0; /* 明細承認FLG追加 2014/12/09 D */
                selected.DeliverDestName = para[22];

                int ritu = 0;
                string sql_str = "select get_kakeritu(:1,:2,:3) 下代掛率,get_kakeritu(:4,:5,:6,1) 社販掛率 from dual";
                var wrk_para = new BizArray();
                wrk_para[0] = Inp12DetailOpt.CustDest.Code;
                wrk_para[1] = "10";
                wrk_para[2] = para[0];
                wrk_para[3] = Inp12DetailOpt.CustDest.Code;
                wrk_para[4] = "10";
                wrk_para[5] = para[0];
                var rit_csv = AppData.Http?.AspxSqlQuery(sql_str, wrk_para.ToArray());
                int wrk_ritu = 0;
                if (rit_csv?.Rows.Count == 0)
                { 
                    selected.BaseRate = int.TryParse(rit_csv.Rows[0][0].ToString(), out var _base_rate) ? _base_rate : 0;
                    selected.Rate = int.TryParse(rit_csv.Rows[0][1].ToString(), out var _rate) ? _rate : 0;
                    wrk_ritu = _base_rate;
                }

                if (selected.CalcRate > 0)
                {
                    ritu = int.TryParse(selected.CalcRate.ToString(), out var _ritu) ? _ritu : 0;
                }
                /* 明細で設定されている場合、明細優先 10.01.28 */
                else if (Inp12DetailOpt.MarkupRate != wrk_ritu && wrk_ritu != 0)
                {
                    ritu = wrk_ritu;
                }
                else if (Inp12DetailOpt.MarkupRate > 0)
                {
                    ritu = int.TryParse(Inp12DetailOpt.MarkupRate.ToString(), out var _ritu) ? _ritu : 0;
                }
                else
                {
                    ritu = wrk_ritu;
                }
                SetKakeRitu(selected, ritu);

                if (ritu >= 0 && ritu != 100)
                {
                    if (SelOrderItem.CostCalcFlg == 0)
                    {
                        double gedai = OnGetGedai(selected.RetailUnitPrice, ritu);
                        selected.WholesalesUnit = int.TryParse(gedai.ToString(), out var tax_excl) ? tax_excl : 0;
                    }
                    else
                    {
                        int total_size = int.TryParse(selected.SizeName.ToString(), out var _size) ? _size : 0;
                        double gedai = OnGetGedai(total_size, ritu);
                        selected.WholesalesUnit = int.TryParse(gedai.ToString(), out var tax_excl) ? tax_excl : 0;
                    }
                }
                else if (SelOrderItem.CostCalcFlg == 0)
                {
                    selected.WholesalesUnit = selected.RetailUnitPrice;
                }
                else {
                    int total_size = int.TryParse(selected.SizeName.ToString(), out var _size) ? _size : 0;
                    selected.WholesalesUnit = total_size;
                } 
            }
            OnChangeValue(selected);
        }
        [RelayCommand]
        public void DoRefresh(Inp12DetailItem value)
        {
            /* 06.03.16 パラメータ変更 */
            var v_wkpara = new BizArray();
            v_wkpara[0] = value.ProdCD; /* 商品CD */
            v_wkpara[1] = value.ColorCD; /* 色 */
            v_wkpara[2] = value.SizeCD; /* サイズ */
            v_wkpara[3] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd"); /* 在庫計上日 */
            v_wkpara[4] = Inp12DetailOpt.StoreCd.Code; /* 倉庫 */
            if (AppData.ClassCvnet.config.jodaihyjflg == 0)
                v_wkpara[4] = "19000101";
            v_wkpara[5] = value.CostFlg.ToString(); /* 原価FLG */
            v_wkpara[6] = Inp12DetailOpt.OrderDate?.ToString("yyyyMMdd"); /* 在庫計上日 */
            v_wkpara[7] = Inp12DetailOpt.CustDest.Code; /* 店舗CD */
            v_wkpara[8] = "10"; /* 取引区分 */
            v_wkpara[9] = value.ProdCD; /* 商品CD */
            v_wkpara[10] = Inp12DetailOpt.CustDest.Code; /* 店舗CD */
            v_wkpara[11] = "10"; /* 取引区分 */
            v_wkpara[12] = value.ProdCD; /* 商品CD */
            v_wkpara[13] = value.ProdCD; /* 商品CD */
            v_wkpara[14] = value.ColorCD; /* 色 */

            var ret_csv = OnQueryTanka(v_wkpara);
            value.DetailName = string.Empty;
            value.RetailUnitPrice = 0;
            value.ColorName = string.Empty;
            value.SizeName = string.Empty;
            int[] wrk_cnt = new int[3];
            wrk_cnt[0] = 0;
            wrk_cnt[1] = 0;
            wrk_cnt[2] = 0;
            for (int i = 0; i < ret_csv.Rows.Count; i++)
            {
                int tanka = int.TryParse(ret_csv.Rows[i][0].ToString(), out int _tanka) ? _tanka : 0;
                if (tanka == 0)
                {
                    wrk_cnt[0] = 1;
                    value.DetailName = ret_csv.Rows[i][1].ToString(); 
                }
            }
        }
        #endregion

        #region Function 
        private void OnChangeValue(Inp12DetailItem row)
        {
            row.RetailAmount = row.Num * row.RetailUnitPrice;
            row.WholesalesAmount = row.Num * row.WholesalesUnit;
            OnChangeTotal(row);
        } 

        private void OnChangeTotal(Inp12DetailItem row)
        {
            if (Inp12DetailItems.Count == 0)
            {
                Inp12DetailOpt.Num = 0;
                Inp12DetailOpt.RetailPrice = 0;
                Inp12DetailOpt.WholesalePrice = 0;
                return;
            }

            int? total_su = 0;
            int? total_kin1 = 0;
            int? total_kin2 = 0; 
            total_su = Inp12DetailItems.Sum(x => x.Num);
            total_kin1 = Inp12DetailItems.Sum(x => x.RetailAmount);
            total_kin2 = Inp12DetailItems.Sum(x => x.WholesalesAmount); 
            foreach (var item in Inp12DetailItems)
            {
                item.UnitPrice = item.WholesalesUnit;
                item.TotalAmount = item.WholesalesAmount;
            }
            Inp12DetailOpt.Num = total_su;
            Inp12DetailOpt.RetailPrice = total_kin1;
            Inp12DetailOpt.WholesalePrice = total_kin2;
        }

        private double OnGetGedai(int? jod, int? kake = null)
        { 
            int keta = int.TryParse( (SelOrderItem.CostDigitCut * -1).ToString(), out var _keta) ? _keta : 0;
            var sritu = Inp12DetailOpt.MarkupRate;
            if (kake != null) {
                sritu = kake;
            }
            if (SelOrderItem.CostRoundType == 1)
            {
                if (SelOrderItem.CostDigitCut == 0)
                    return GlobalFunc.RoundUp((double)(jod * sritu) / 100);
                else
                    return GlobalFunc.RoundUp((double)(jod * sritu) / 100, keta);
            }
            else if (SelOrderItem.CostRoundType == 2)
            {
                if (SelOrderItem.CostDigitCut == 0)
                    return GlobalFunc.RoundDown((double)(jod * sritu) / 100);
                else
                    return GlobalFunc.RoundDown((double)(jod * sritu) / 100, keta);
            }
            else
            {
                if (SelOrderItem.CostDigitCut == 0)
                    return Math.Round((double)(jod * sritu) / 100);
                else
                    return Math.Round((double)(jod * sritu) / 100, keta);
            }
        } 

        private void OnCalcKakeRitu(Inp12DetailItem p_row)
        {
            var jodai = p_row.RetailUnitPrice; //上代単価
            var gedai = p_row.WholesalesUnit;  //下代単価

            int ritu = 0;

            if (jodai > 0)
            {
                double value = (double)(gedai / jodai) + 0.009;
                ritu = (int)Math.Truncate((value) * 100) / 100;
            }
            SetKakeRitu(p_row, ritu);
        }

        private void SetKakeRitu(Inp12DetailItem p_row, int p_ritu)
        {
            p_row.CalcRate = p_ritu;
            var a = decimal.TryParse(p_row.BaseRate.ToString(), out var valueA) ? valueA : 0; 
            var b = decimal.TryParse(p_ritu.ToString(), out var valueB) ? valueB : 0;

            if (Math.Truncate(a) == Math.Truncate(b))
            {
                /* マスタ掛率と計算掛率が違う場合は背景色を変更 */
                p_row.CalcRateBgColor = Brushes.White;
                p_row.CalcRateFgColor = Brushes.Black;
            }
            else
            {
                p_row.CalcRateFgColor = Brushes.White;
                p_row.CalcRateBgColor = Brushes.Red;
            }
        }

        /* マスタチェック集約 */
        private int OnCheckMst()
        {
            if (Inp12DetailOpt == null) return -1;
            /* 倉庫チェック */ 
            if (Inp12DetailOpt.StoreCd.Display != "" && Inp12DetailOpt.StoreCd.pre_data != null && Inp12DetailOpt.StoreCd.pre_data.Code != Inp12DetailOpt.StoreCd.Code)
            { 
                string v_sqlstr = "select 得意先CD,得意先名 from HC$MASTER_TOKUI ";
                v_sqlstr += " where 得意先CD =:1 and (店種区分=0 OR 倉庫区分=9)";
                v_sqlstr += AppData.ClassCvnet.GetQueryStrHoujin();	/* 2010.09.22　法人CD対応 */
                var v_array = new BizArray();
                v_array[0] = Inp12DetailOpt.StoreCd.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv.Rows.Count > 0)
                {
                    Inp12DetailOpt.StoreCd = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp12DetailOpt.StoreCd.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp12DetailOpt.StoreCd = new BtListHelper("", "");
                    System.Windows.MessageBox.Show("倉庫CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            /* 得意先チェック */
            /* 2017.09.15 問合せ_#37030対応 */
            if (Inp12DetailOpt.CustDest.Code.Trim() != "" && Inp12DetailOpt.CustDest.pre_data != null && Inp12DetailOpt.CustDest.pre_data.Code != Inp12DetailOpt.CustDest.Code)
            {
                /* 20161206 チェックが一覧ボタンと同じものになるよう変更 */
                string v_sqlstr = "select 得意先CD,得意先名,掛率,セール掛率,消費税CD,消費税計算方法,消費税端数,下代桁切指定,下代端数区分,下代計算FLG"
                    + ", 営業担当CD||' '||NVL((select S.名前 from HC$MASTER_SHAIN S where S.社員CD=営業担当CD),'') 営業担当 from HC$master_TOKUI where 得意先CD=:1 and 店種区分 >0 " + 
                    AppData.ClassCvnet.GetQueryStrHoujin();
                var v_array = new BizArray();
                v_array[0] = Inp12DetailOpt.CustDest.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv.Rows.Count > 0)
                {
                    Text30 = int.TryParse(wrk_csv.Rows[0][2].ToString(), out var text30) ? text30 : 0;
                    Text31 = int.TryParse(wrk_csv.Rows[0][3].ToString(), out var text31) ? text31 : 0;
                    Text32 = int.TryParse(wrk_csv.Rows[0][4].ToString(), out var text32) ? text32 : 0;
                    Text33 = int.TryParse(wrk_csv.Rows[0][5].ToString(), out var text33) ? text33 : 0;
                    Text34 = int.TryParse(wrk_csv.Rows[0][6].ToString(), out var text34) ? text34 : 0;
                    Text35 = int.TryParse(wrk_csv.Rows[0][7].ToString(), out var text35) ? text35 : 0;
                    Text36 = int.TryParse(wrk_csv.Rows[0][8].ToString(), out var text36) ? text36 : 0;
                    Text37 = int.TryParse(wrk_csv.Rows[0][9].ToString(), out var text37) ? text37 : 0;
                    /* 営業担当追加 */
                    string salesRep = wrk_csv.Rows[0][10].ToString();
                    if (salesRep.Trim() != "." && !string.IsNullOrEmpty(salesRep.Trim()))
                    {
                        string[] sales = salesRep.Split(' ');
                        if (sales.Length > 1)
                            Inp12DetailOpt.SalesRep = new BtListHelper(sales[0], sales[1]);
                    }
                    Inp12DetailOpt.MarkupRate = float.TryParse(Text30.ToString(), out var mark_rate) ? mark_rate : 0;
                    Inp12DetailOpt.CustDest = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp12DetailOpt.CustDest.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp12DetailOpt.CustDest = new BtListHelper("","");
                    System.Windows.MessageBox.Show("得意先CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            /* 入力者チェック 21.10.13 */
            if (Inp12DetailOpt.InpCd.Code.Trim() != "" && Inp12DetailOpt.InpCd.Code != "." && 
                Inp12DetailOpt.InpCd.pre_data != null && Inp12DetailOpt.InpCd.pre_data.Code != Inp12DetailOpt.InpCd.Code)
            {
                string v_sqlstr = "select 社員CD,名前 from HC$MASTER_SHAIN where 社員CD=:1";
                var v_array = new BizArray();
                v_array[0] = Inp12DetailOpt.InpCd.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv.Rows.Count > 0)
                {
                    Inp12DetailOpt.InpCd = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp12DetailOpt.InpCd.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp12DetailOpt.InpCd = new BtListHelper("", "");
                    System.Windows.MessageBox.Show("入力者CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            /* 営業担当チェック 21.10.13 */
            if (Inp12DetailOpt.SalesRep.Code.Trim() != "" && Inp12DetailOpt.SalesRep.Code != "." && 
                Inp12DetailOpt.SalesRep.pre_data != null && Inp12DetailOpt.SalesRep.pre_data.Code != Inp12DetailOpt.SalesRep.Code)
            {
                string v_sqlstr = "select 社員CD,名前 from HC$MASTER_SHAIN where 社員CD=:1";
                var v_array = new BizArray();
                v_array[0] = Inp12DetailOpt.SalesRep.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv.Rows.Count > 0)
                {
                    Inp12DetailOpt.SalesRep = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp12DetailOpt.SalesRep.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp12DetailOpt.SalesRep = new BtListHelper("", "");
                    System.Windows.MessageBox.Show("営業担当者CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            return 0;
        }

        private void OnSetMultiRow(List<BizArray> set_para, string v_para)
        {
            /* データ列の並びを固定するようにする */
            foreach (var para in set_para)
            {
                Inp12DetailItem item = new Inp12DetailItem();
                item.ProdCD = para[0]; /* 商品CD */
                item.DetailName = para[1]; /* 商品名 */
                item.ColorCD = para[13]; /* 色CD */
                item.SizeCD = para[14]; /* サイズCD */
                item.ColorName = para[15]; /* 色名 */
                item.SizeName = para[16]; /* サイズ名 */
                item.Num = int.TryParse(para[18], out int _num) ? _num : 0; /* 数量 */

                item.RetailUnitPrice = int.TryParse(para[23], out int _retail_unit) ? _retail_unit : 0; /* 上代 */
                item.SizeName = para[25]; /* マスタ上代 */
                item.TaxCalcMethod = short.TryParse(para[9], out short _cal_method) ? _cal_method : (short)0; /* 消費税計算方法 */
                item.BaseRate = int.TryParse(para[28], out int _rate) ? _rate : 0; /* マスタ掛率 */
                item.DeliverDate = para[32]; /* 納品日 */
                item.DM1 = item.DeliverDate; /* 納品日(表示用) */
                item.ApproveFlg = int.TryParse(para[33], out int _approve) ? _approve : 0; /* 明細承認FLG */
                item.DeliverDestName = item.ApproveFlg.ToString(); /* 明細承認FLG(表示用) */

                item.CostFlg = int.TryParse(para[35], out int _costflg) ? _costflg : 0; /* 原価FLG */
                /* 下代単価計算 */
                if (v_para == "1")
                {
                    /* 品番選択の場合、下代単価 */
                    item.WholesalesUnit = int.TryParse(para[24], out int _whole_unit) ? _whole_unit : 0;
                    /* 上代下代から掛率を算出 */
                    OnCalcKakeRitu(item);
                }
                else
                {
                    /* 展開・バーコードの場合、上代金額＊掛率 */
                    if (para.Count < 30 || v_para == "3")
                    {
                        int? jod = int.TryParse(para[23], out int _jod) ? _jod : 0;
                        int? kake = int.TryParse(para[23], out int _kake) ? _kake : 0;
                        item.WholesalesUnit = int.TryParse(OnGetGedai(jod, kake).ToString(), out int _unit) ? _unit : 0;
                        int p_ritu = int.TryParse(para[28], out int _ritu) ? _ritu : 0;
                        SetKakeRitu(item, p_ritu);
                    }
                    else
                    {
                        item.WholesalesUnit = int.TryParse(para[30], out int _unit) ? _unit : 0;
                        int p_ritu = int.TryParse(para[31], out int _ritu) ? _ritu : 0;
                        SetKakeRitu(item, p_ritu);
                    }
                }

                /* 明細合計計算 */
                item.UnitPrice = item.WholesalesUnit;
                item.RetailAmount = item.Num * item.RetailUnitPrice;
                item.WholesalesAmount = item.Num * item.WholesalesUnit;
                item.TotalAmount = item.Num * item.UnitPrice;

                item.CompleteFlg = 0;

                OnChangeTotal(item); 
                Inp12DetailItems.Add(item);
            }
        }

        /*********************/
        /* 単価検索処理 */
        /*********************/
        private DataTable OnQueryTanka(BizArray v_wkpara)
        {
            /* 05.03.16 上代、原価、サイズ名変更 */
            /* 08.09.26 納品日追加 */
            string sql_query = "select 0,";
            sql_query += "商品名 商品名,";
            sql_query += "GET_JODAI(:0,:1,:2,:3,:4) 上代,GET_GENKA(商品CD,:5,:6) 原価,消費税計算方法";
            sql_query += " ,to_number(get_kakeritu(:7,:8,:9)) 下代掛率,to_number(get_kakeritu(:10,:11,:12,1)) 社販掛率";
            sql_query += " ,納品日";
            sql_query += " from HC$MASTER_SHOHIN where 商品CD=:13 ";
            /* 2012.05.28 USER87は海外名で */
            sql_query += " union select 1,名称,0,0,0,0,0,'19010101' from (select 名称CD,名称 from HC$Master_MEISHO where 名称区分='COL') where 名称CD=:14 ";
            sql_query += " union select 2,名称,0,0,0,0,0,'19010101' from (select GET_SIZENAME(:0,:2) 名称 from dual)";
            var ret_csv = AppData.Http.AspxSqlQuery(sql_query, v_wkpara.ToArray());
            return ret_csv;
        }
        #endregion 
    }

    public partial class Inp12SearchOpt : ObservableObject
    {
        /// <summary>
        /// 伝票No - START
        /// </summary>
        [ObservableProperty]
        string? m_SlipStartNo;

        /// <summary>
        /// 伝票No - END
        /// </summary>
        [ObservableProperty]
        string? m_SlipEndNo;

        /// <summary>
        /// 受注日 - START
        /// </summary>
        [ObservableProperty]
        DateTime? m_OrderStartDate;

        /// <summary>
        /// 受注日 - END
        /// </summary>
        [ObservableProperty]
        DateTime? m_OrderEndDate;

        /// <summary>
        /// 取引区分 - START
        /// </summary>
        [ObservableProperty]
        string? m_TranStartCat;

        /// <summary>
        /// 取引区分 - END
        /// </summary>
        [ObservableProperty]
        string? m_TranEndCat;

        /// <summary>
        /// 関連No1 - START
        /// </summary>
        [ObservableProperty]
        string? m_RelatedStartNo;

        /// <summary>
        /// 関連No1 - END
        /// </summary>
        [ObservableProperty]
        string? m_RelatedEndNo;

        /// <summary>
        /// 関連No2 - START
        /// </summary>
        [ObservableProperty]
        string? m_RelatedStartNo2;

        /// <summary>
        /// 関連No2 - END
        /// </summary>
        [ObservableProperty]
        string? m_RelatedEndNo2;

        /// <summary>
        /// 手入力No - START
        /// </summary>
        [ObservableProperty]
        string? m_ManualInpStartNo;

        /// <summary>
        /// 手入力No - END
        /// </summary>
        [ObservableProperty]
        string? m_ManualInpEndNo;

        /// <summary>
        /// 得意先 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_CustStartDest;

        /// <summary>
        /// 得意先 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_CustEndDest;

        /// <summary>
        /// 倉庫 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_StoreStartCd;

        /// <summary>
        /// 倉庫 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_StoreEndCd;

        /// <summary>
        /// 商品 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ProdStartCd;

        /// <summary>
        /// 商品 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ProdEndCd;

        /// <summary>
        /// 展示会 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ExhibitStartCd;

        /// <summary>
        /// 展示会 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ExhibitEndCd;

        /// <summary>
        /// 入力者 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_InpStartCd;

        /// <summary>
        /// 入力者 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_InpEndCd;

        /// <summary>
        /// 行 Option (0: 一覧, 1: 明細)
        /// </summary>
        [ObservableProperty]
        int m_SelPrintCond;

        public Inp12SearchOpt()
        {
            SlipStartNo = "0";
            SlipEndNo = "9999999999";
            OrderStartDate = new DateTime(1901, 1, 1);
            OrderEndDate = new DateTime(2099, 12, 31);
            TranStartCat = "00";
            TranEndCat = "99";
            RelatedStartNo = "0";
            RelatedEndNo = "9999999999999";
            RelatedStartNo2 = "0";
            RelatedEndNo2 = "9999999999999";
            ManualInpStartNo = "";
            ManualInpEndNo = "ﾟ";
            CustStartDest = new BtListHelper("", "");
            CustEndDest = new BtListHelper("99999999", "");
            StoreStartCd = new BtListHelper("", "");
            StoreEndCd = new BtListHelper("99999999", "");
            ProdStartCd = new BtListHelper("", "");
            ProdEndCd = new BtListHelper("zzzzzzzzzzzzzzzzzzzz", "");
            ExhibitStartCd = new BtListHelper("", "");
            ExhibitEndCd = new BtListHelper("99999999", "");
            InpStartCd = new BtListHelper("", "");
            InpEndCd = new BtListHelper("99999999", "");
            SelPrintCond = 0;
        }
    }

    public partial class SelOrderTran : ObservableObject
    {
        /// <summary>
        /// SEQ_NO
        /// </summary>
        [ObservableProperty]
        int? m_SeqNo;

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
        /// 関連伝票NO
        /// </summary>
        [ObservableProperty]
        long? m_RelatedSlipNo;

        /// <summary>
        /// 在庫計上日
        /// </summary>
        [ObservableProperty]
        string? m_InvCountDate;

        /// <summary>
        /// 納品日
        /// </summary>
        [ObservableProperty]
        string? m_DeliverDate;

        /// <summary>
        /// 取引区分
        /// </summary>
        [ObservableProperty]
        string? m_TranCate;

        /// <summary>
        /// 入力社員CD
        /// </summary>
        [ObservableProperty]
        string? m_InpStaffCD;

        /// <summary>
        /// 倉庫CD
        /// </summary>
        [ObservableProperty]
        string? m_StoreCD;

        /// <summary>
        /// 取引先CD1
        /// </summary>
        [ObservableProperty]
        string? m_ClientCD1;

        /// <summary>
        /// 掛率1
        /// </summary>
        [ObservableProperty]
        float? m_MarkupRate1;

        /// <summary>
        /// 外税対象金額
        /// </summary>
        [ObservableProperty]
        long? m_TaxableAmount;

        /// <summary>
        /// 数量合計
        /// </summary>
        [ObservableProperty]
        double? m_TotalNum;

        /// <summary>
        /// 明細金額合計
        /// </summary>
        [ObservableProperty]
        long? m_TotalDetailAmt;

        /// <summary>
        /// 内税消費税
        /// </summary>
        [ObservableProperty]
        long? m_TaxIncluded;

        /// <summary>
        /// 外税消費税
        /// </summary>
        [ObservableProperty]
        long? m_TaxExcluded;

        /// <summary>
        /// 上代合計
        /// </summary>
        [ObservableProperty]
        long? m_TotalRetail;

        /// <summary>
        /// 下代合計
        /// </summary>
        [ObservableProperty]
        long? m_TotalWholesale;

        /// <summary>
        /// メモ
        /// </summary>
        [ObservableProperty]
        string? m_Memo;

        /// <summary>
        /// 掛率2
        /// </summary>
        [ObservableProperty]
        float? m_MarkupRate2;

        /// <summary>
        /// 伝票処理区分
        /// </summary>
        [ObservableProperty]
        short? m_SlipCate;

        /// <summary>
        /// MOD_SEQ
        /// </summary>
        [ObservableProperty]
        long? m_ModSeq;

        /// <summary>
        /// 関連伝票NO2
        /// </summary>
        [ObservableProperty]
        long? m_RelatedSlipNo2;

        /// <summary>
        /// 展示会CD
        /// </summary>
        [ObservableProperty]
        string? m_ExhibitCD;

        /// <summary>
        /// 手入力伝票NO
        /// </summary>
        [ObservableProperty]
        string? m_ManualInpNo;

        /// <summary>
        /// 納品先CD
        /// </summary>
        [ObservableProperty]
        string? m_ShipToCD;

        /// <summary>
        /// 担当者CD
        /// </summary>
        [ObservableProperty]
        string? m_StaffCD;

        /// <summary>
        /// 入力社員名
        /// </summary>
        [ObservableProperty]
        string? m_InpStaffName;

        /// <summary>
        /// 得意先名
        /// </summary>
        [ObservableProperty]
        string? m_TradeName;

        /// <summary>
        /// 倉庫名
        /// </summary>
        [ObservableProperty]
        string? m_StoreName;

        /// <summary>
        /// 掛率
        /// </summary>
        [ObservableProperty]
        float? m_MarkupRate;

        /// <summary>
        /// セール掛率
        /// </summary>
        [ObservableProperty]
        float? m_SalesRate;

        /// <summary>
        /// 消費税CD
        /// </summary>
        [ObservableProperty]
        long? m_TaxCD;

        /// <summary>
        /// 消費税計算方法
        /// </summary>
        [ObservableProperty]
        short? m_TaxCalcMethod;

        /// <summary>
        /// 消費税端数
        /// </summary>
        [ObservableProperty]
        short? m_TaxRound;

        /// <summary>
        /// 下代桁切指定
        /// </summary>
        [ObservableProperty]
        short? m_CostDigitCut;

        /// <summary>
        /// 下代端数区分
        /// </summary>
        [ObservableProperty]
        short? m_CostRoundType;

        /// <summary>
        /// 下代計算FLG
        /// </summary>
        [ObservableProperty]
        short? m_CostCalcFlg;

        /// <summary>
        /// 完了FLG
        /// </summary>
        [ObservableProperty]
        short? m_CompleteFlg;

        /// <summary>
        /// 展示会名
        /// </summary>
        [ObservableProperty]
        string? m_ExhibitName;

        /// <summary>
        /// 納品先名
        /// </summary>
        [ObservableProperty]
        string? m_ShipToName;

        /// <summary>
        /// 担当者名
        /// </summary>
        [ObservableProperty]
        string? m_StaffName;
    }

    public partial class Inp12DetailOpt : ObservableObject
    {
        /// <summary>
        /// 伝票No
        /// </summary>
        [ObservableProperty]
        string? m_SlipNo;

        /// <summary>
        /// 受注日
        /// </summary>
        [ObservableProperty]
        DateTime? m_OrderDate;

        /// <summary>
        /// 納品日
        /// </summary>
        [ObservableProperty]
        DateTime? m_DeliverDate;

        /// <summary>
        /// 取引区分
        /// </summary>
        [ObservableProperty]
        string? m_TranCate;

        /// <summary>
        /// 掛率
        /// </summary>
        [ObservableProperty]
        float? m_MarkupRate;

        /// <summary>
        /// 得意先
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_CustDest;

        /// <summary>
        /// 関連No1
        /// </summary>
        [ObservableProperty]
        string? m_RelatedNo;

        /// <summary>
        /// 関連No2
        /// </summary>
        [ObservableProperty]
        string? m_RelatedNo2;

        /// <summary>
        /// 手入力No
        /// </summary>
        [ObservableProperty]
        string? m_ManualInpNo;

        /// <summary>
        /// 倉庫
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_StoreCd;

        /// <summary>
        /// 入力者
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_InpCd;

        /// <summary>
        /// 展示会
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_ExhibitCd;

        /// <summary>
        /// 備考
        /// </summary>
        [ObservableProperty]
        string? m_Memo;

        /// <summary>
        /// 営業担当
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SalesRep;

        [ObservableProperty]
        BtListHelper? m_ProdCd;

        /// <summary>
        /// 計 (数量)
        /// </summary>
        [ObservableProperty]
        int? m_Num;

        /// <summary>
        /// 計 (上代金額)
        /// </summary>
        [ObservableProperty]
        int? m_RetailPrice;

        /// <summary>
        /// 計 (下代金額)
        /// </summary>
        [ObservableProperty]
        int? m_WholesalePrice;

        /// <summary>
        /// 作成日
        /// </summary>
        [ObservableProperty]
        string? m_CreatedAt;

        /// <summary>
        /// 修正日
        /// </summary>
        [ObservableProperty]
        string? m_UpdatedAt;
    }

    public partial class Inp12DetailItem : ObservableObject
    {
        /// <summary>
        /// SEQ_NO
        /// </summary>
        [ObservableProperty]
        int? m_SeqNo;

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
        /// 明細取引区分
        /// </summary>
        [ObservableProperty]
        short? m_TranCate;

        /// <summary>
        /// 商品CD
        /// </summary>
        [ObservableProperty]
        string? m_ProdCD;

        /// <summary>
        /// 色CD
        /// </summary>
        [ObservableProperty]
        string? m_ColorCD;

        /// <summary>
        /// サイズCD
        /// </summary>
        [ObservableProperty]
        string? m_SizeCD;

        /// <summary>
        /// 明細名称
        /// </summary>
        [ObservableProperty]
        string? m_DetailName;

        /// <summary>
        /// 数量
        /// </summary>
        [ObservableProperty]
        int? m_Num;

        /// <summary>
        /// 単価
        /// </summary>
        [ObservableProperty]
        int? m_UnitPrice;

        /// <summary>
        /// 金額
        /// </summary>
        [ObservableProperty]
        int? m_TotalAmount;

        /// <summary>
        /// 内税消費税
        /// </summary>
        [ObservableProperty]
        long? m_TaxIncluded;

        /// <summary>
        /// 外税消費税
        /// </summary>
        [ObservableProperty]
        long? m_TaxExcluded;

        /// <summary>
        /// 上代単価
        /// </summary>
        [ObservableProperty]
        int? m_RetailUnitPrice;

        /// <summary>
        /// 上代金額
        /// </summary>
        [ObservableProperty]
        int? m_RetailAmount;

        /// <summary>
        /// 下代単価
        /// </summary>
        [ObservableProperty]
        int? m_WholesalesUnit;

        /// <summary>
        /// 下代金額
        /// </summary>
        [ObservableProperty]
        int? m_WholesalesAmount;

        /// <summary>
        /// 明細メモ
        /// </summary>
        [ObservableProperty]
        string? m_DetailMemo;

        /// <summary>
        /// 消費税計算方法
        /// </summary>
        [ObservableProperty]
        short? m_TaxCalcMethod;

        /// <summary>
        /// 商品シリアル
        /// </summary>
        [ObservableProperty]
        string? m_ProdSerial;

        /// <summary>
        /// 関連伝票NO
        /// </summary>
        [ObservableProperty]
        long? m_RelatedSlipNo;

        /// <summary>
        /// 関連伝票行NO
        /// </summary>
        [ObservableProperty]
        long? m_RelatedSlipLineNo;

        /// <summary>
        /// JANCODE
        /// </summary>
        [ObservableProperty]
        string? m_Jancode;

        /// <summary>
        /// 原価FLG
        /// </summary>
        [ObservableProperty]
        int? m_CostFlg;

        /// <summary>
        /// 完了FLG
        /// </summary>
        [ObservableProperty]
        int? m_CompleteFlg;

        /// <summary>
        /// 納品日
        /// </summary>
        [ObservableProperty]
        string? m_DeliverDate;

        /// <summary>
        /// 明細承認FLG
        /// </summary>
        [ObservableProperty]
        int? m_ApproveFlg;

        /// <summary>
        /// COL名
        /// </summary>
        [ObservableProperty]
        string? m_ColorName;

        /// <summary>
        /// サイズ名
        /// </summary>
        [ObservableProperty]
        string? m_SizeName;

        /// <summary>
        /// 上代
        /// </summary>
        [ObservableProperty]
        decimal? m_Retail;

        /// <summary>
        /// 下代掛率
        /// </summary>
        [ObservableProperty]
        string? m_WholesalesRate;

        /// <summary>
        /// DM1
        /// </summary>
        [ObservableProperty]
        string? m_DM1;

        /// <summary>
        /// スワッチ頁
        /// </summary>
        [ObservableProperty]
        string? m_SwatchPage;

        /// <summary>
        /// スワッチ位置
        /// </summary>
        [ObservableProperty]
        string? m_SwatchPosition;

        /// <summary>
        /// 納品先名
        /// </summary>
        [ObservableProperty]
        string? m_DeliverDestName;

        /// <summary>
        /// 担当者名
        /// </summary>
        [ObservableProperty]
        string? m_StaffName;

        /// <summary>
        /// マスタ掛率 Dsp80
        /// </summary>
        [ObservableProperty]
        int? m_BaseRate;

        /// <summary>
        /// 掛率 Dsp81
        /// </summary>
        [ObservableProperty]
        int? m_Rate;

        /// <summary>
        /// 計算掛率 Dsp82
        /// </summary>
        [ObservableProperty]
        int? m_CalcRate;

        /// <summary>
        /// 計算掛率 Dsp82 BackgroundColor
        /// </summary>
        [ObservableProperty]
        Brush? m_CalcRateBgColor;

        /// <summary>
        /// 計算掛率 Dsp82 ForegroundColor
        /// </summary>
        [ObservableProperty]
        Brush? m_CalcRateFgColor;

        [ObservableProperty]
        bool? m_IsReadOnly;

        public Inp12DetailItem()
        {
            CalcRateBgColor = Brushes.White;
            CalcRateFgColor = Brushes.Black;
            IsReadOnly = true;
        }
    }
}
