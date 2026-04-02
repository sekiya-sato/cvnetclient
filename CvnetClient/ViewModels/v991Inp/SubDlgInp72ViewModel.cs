using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Class;
using CvnetClient.Models;
using CvnetClient.Utils;
using CvnetClient.Views;
using System.Collections.ObjectModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Media;

namespace CvnetClient.ViewModels
{
    public partial class SubDlgInp72ViewModel : BaseViewModel
    {
        #region Variables
        [ObservableProperty]
        public int? m_SelectedTabIndex;

        [ObservableProperty]
        public string? m_Mess1;

        [ObservableProperty]
        public string? m_Mess2;

        [ObservableProperty]
        public int? m_PrdToggle = 0;

        [ObservableProperty]
        public Inp72SearchOpt m_Inp72SearchOpt;

        [ObservableProperty]
        public ObservableCollection<Inp72TranHead> m_Inp72TranHeads;

        [ObservableProperty]
        public Inp72TranHead m_Inp72TranHeadItem;
        #endregion

        #region Private Variables
        private int Chg_flg = 0;
        private int Max_Text = 22;
        private int Max_Line = 24;/* 名称除いた最大明細列 */
        /* 2016.10.13 CSVフラグ追加 */
        private int CSV_flg = 0;

        private BizArray col_list; /* Insert,Update時の項目名リスト */
        private BizArray col_list2; /* 明細の項目名リスト */
        private string sql_collist = "手入力伝票NO,在庫計上日,納品日,取引区分,入力社員CD,取引先CD2,"
                                   + "取引先CD1,掛率1,外税対象金額,数量合計,明細金額合計,"
                                   + "内税消費税,外税消費税,上代合計,下代合計,メモ,掛計上FLG,伝票処理区分,MOD_SEQ,倉庫CD,関連伝票NO"
                                   + ",関連伝票NO2";
        private string sql_collist2 = "明細取引区分,商品CD,色CD,サイズCD,明細名称,数量,単価,金額,内税消費税,外税消費税,"
                                    + "上代単価,上代金額,下代単価,下代金額,明細メモ,消費税計算方法"
                                    + ",商品シリアル,関連伝票NO,関連伝票行NO,JANCODE,原価FLG"
                                    + ",完了FLG";

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

        public void OnInit(object? init_para = null)
        {
            OnInitBase(init_para);
            Inp72SearchOpt = new Inp72SearchOpt();
            Inp72DetailOpt = new Inp72DetailOpt();

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

            var get_cats = comboItem.ComboItem_00<string>("商品受注区分");
            var set_cats = new Dictionary<string, string>();
            set_cats.Add("00", "00");
            foreach (var cat in get_cats)
            {
                set_cats.Add(cat.Key, cat.Value);
            }
            set_cats.Add("99", "99");
            TranCatList = set_cats;
            Inp72SearchOpt.TranStartCat = TranCatList.FirstOrDefault().Key;
            Inp72SearchOpt.TranEndCat = TranCatList.LastOrDefault().Key;

            var status = new Dictionary<int, string>
            {
                { 0 ,"0 未完" },
                { 1 ,"1 完了" }
            };
            StatusList = status;
            #endregion

            /* 初期化処理 */
            Inp72SearchOpt.DeliverStartDate = AppData.ClassCvnet.ImpDateDiff.OnGetDay();
            Inp72DetailOpt.OrderDate = DateTime.Now;
            Inp72DetailOpt.DeliverDate = DateTime.Now;
            Inp72DetailOpt.TranCate = TranCatList?.Where(x => x.Key == "10").FirstOrDefault().Key ?? "10";
            Inp72DetailOpt.InpCd = new BtListHelper(AppData.ClassSatoo.SHAIN_CD, AppData.ClassSatoo.SHAIN_Name);
            /* 20041111 Text7Length over のエラーを修正 */
            var ret_csv = AppData.ClassCvnet.AspxSqlQueryImp();
            Inp72DetailOpt.RecDestCd = new BtListHelper(ret_csv.Rows[0][1].ToString(), ret_csv.Rows[0][2].ToString());

            /* 2021.02.18 #56430対応追加  */
            if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
            {
                Inp72SearchOpt.CsvExportTitle = "EXCEL出力";
            }
        }

        #region Dialog Search
        /// <summary>
        /// 仕入先 Search Dialog
        /// </summary> 
        [RelayCommand]
        private void SelSupplierCD01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.SupplierStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp72SearchOpt.SupplierEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelSupplierCD02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.SupplierEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        /// <summary>
        /// 入庫先 Search Dialog
        /// </summary> 
        [RelayCommand]
        private void SelRecDestCD01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.RecDestStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp72SearchOpt.RecDestEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelRecDestCD02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.RecDestEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        /// <summary>
        /// 商品 Search Dialog
        /// </summary>
        [RelayCommand]
        private void SelProdCD01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.ProdStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp72SearchOpt.ProdEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelProdCD02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.ProdEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        /// <summary>
        /// 入力者 Search Dialog
        /// </summary> 
        [RelayCommand]
        private void SelInpStaffCD01(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.InpStartCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
                Inp72SearchOpt.InpEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelInpStaffCD02(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72SearchOpt != null)
            {
                Inp72SearchOpt.InpEndCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// 商品 ToggleButton
        /// </summary>
        [RelayCommand]
        private void SelPrdToggle()
        {
            if (PrdToggle == 0) PrdToggle = 1;
            else PrdToggle = 0;
        }

        /// <summary>
        /// 一覧取得 PrevPage
        /// </summary>
        [RelayCommand]
        private void DoPrevButton()
        {
            if (Inp72TranHeads.Count == 0) return;

            /* パラメータ生成 */
            CreatePara();

            /* 20210205 51-ページ数を表示 */
            Inp72TranHead select_tran = Inp72TranHeads.FirstOrDefault();
            if (select_tran != null)
                OnQuery(param1.ToArray(), param2.ToArray(), select_tran.SeqNo.ToString(), "asc");
        }

        /// <summary>
        /// 一覧取得(F5)
        /// </summary>
        [RelayCommand]
        private void DoSearch()
        {
            /* パラメータ生成 */
            CreatePara();

            /* 20210205 ページ表示機能 */
            OnQuery(param1.ToArray(), param2.ToArray(), "");
        }

        /// <summary>
        /// 一覧取得 NextPage
        /// </summary>
        [RelayCommand]
        private void DoNextButton()
        {
            if (Inp72TranHeads.Count == 0) return;

            /* パラメータ生成 */
            CreatePara();

            /* 20210205 51-ページ数を表示 */
            Inp72TranHead select_tran = Inp72TranHeads.LastOrDefault();
            if (select_tran != null)
                OnQuery(param1.ToArray(), param2.ToArray(), select_tran.SeqNo.ToString());
        }

        [RelayCommand]
        private async Task DoPrint()
        {
            CSV_flg = 0;

            /* パラメータ生成 */
            CreatePara();

            Mess2 = "スプール中です";
            var ret_csv = "";
            if (Inp72SearchOpt.SelPrintCond == 0)
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
        private async Task DoCSV()
        {
            CSV_flg = 1;
            /* パラメータ生成 */
            CreatePara();
            Mess2 = "出力中です";
            var ret_csv = "";
            if (Inp72SearchOpt.SelPrintCond == 0)
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

            var ret_name = AppData.Http!.AspxSqlQuery("select m.名称 from HC$MASTER_MEISHO m where m.名称区分='IDX' and m.名称CD between 'B01' and 'B10' order by m.名称CD");
            var name_cnt = 0;
            var name_flg = 0;

            string datapath = AppData.Http.URLroot + pdfPath + "/data.txt";
            string headpath = AppData.Http.URLroot + pdfPath + "/d_sql.txt";
            bool ready = await Utils.GlobalFunc.WaitForPdfAsync(datapath, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            ready = await Utils.GlobalFunc.WaitForPdfAsync(headpath, TimeSpan.FromSeconds(30));
            if (!ready)
            {
                ClientLib.MessageBoxError(this, "Header生成に時間がかかりすぎています。\n 条件を絞ってください。");
                return;
            }

            var header_csv = new BizCsvDocument();
            await header_csv.LoadHeaderFromUrl(headpath);
            var get_csv = new BizCsvDocument();
            await get_csv.LoadFromUrlAsync(datapath, headpath);
            var dt = get_csv.GetTable();
            var dt_header = header_csv.GetTable();
            dt.Rows.InsertAt(dt.NewRow(), 0);

            for (var i = 0; i < dt.Columns.Count; i++)
            {
                /* 2021.04.27 商品名称CDのラベル付け */

                string header_substr = dt_header.Rows[0][i].ToString();
                if (header_substr.Length > 3)
                {
                    header_substr = dt_header.Rows[0][i].ToString().Substring(0, 4);
                }
                if (Inp72SearchOpt.SelPrintCond == 1 && header_substr == "名称CD" && ret_name.Rows.Count == 10)
                {
                    if (name_flg == 0)
                    {
                        dt.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "CD";
                        name_flg = 1;
                    }
                    else
                    {
                        dt.Rows[0][i] = ret_name.Rows[name_cnt][0].ToString() + "名";
                        name_cnt++;
                        name_flg = 0;
                    }
                }
                else
                {
                    dt.Rows[0][i] = dt_header.Rows[0][i];
                }
            }
            get_csv = new BizCsvDocument(dt);
            var str = get_csv.SaveStr(1);
            get_csv = new BizCsvDocument(str, 1);

            /* 2021.02.18 #56430対応修正  */
            if (AppData.ClassCvnet.config.ExcelOutFlg == 1)
            {
                var dtCSVFlNm = "";
                var ExcelFlNm = "";

                try
                {

                    if (Inp72SearchOpt.SelPrintCond == 0)
                    {
                        dtCSVFlNm = "DataIchiran.csv";
                        ExcelFlNm = "Ichiran_macro.xlsm";
                    }
                    else if (Inp72SearchOpt.SelPrintCond == 1)
                    {
                        dtCSVFlNm = "DataMeisai.csv";
                        ExcelFlNm = "Meisai_macro.xlsm";
                    }

                    get_csv.SaveCsv(dtCSVFlNm);
                    Mess2 = "データを保存しました";
                }
                catch (Exception ex)
                {
                    Mess2 = "保存を中止しました";
                }
            }
            else
            {

                var v_title = "受注伝票";
                try
                {
                    string f_name = (Inp72SearchOpt.SelPrintCond == 0) ? "一覧" : "明細";
                    get_csv.SaveCsv(v_title + f_name);
                    Mess2 = "データを保存しました";
                }
                catch (Exception ex)
                {
                    Mess2 = "保存を中止しました";
                }
            }
        }
        /// <summary>
        /// 修正・照会(F6)
        /// </summary>
        [RelayCommand]
        private void RowDoubleClick(Inp72TranHead item)
        {
            if (item == null) item = Inp72TranHeadItem;
            if (item == null) return;
            if (Inp72DetailOpt == null) Inp72DetailOpt = new Inp72DetailOpt();
            Inp72DetailOpt.SlipNo = item.SeqNo.ToString();
            double created = double.TryParse(item.VdateCreate.ToString(), out created) ? created : 0;
            Inp72DetailOpt.CreatedAt = AppData.ClassSatoo.GetVdate(created).ToString();
            double updated = double.TryParse(item.VdateUpdate.ToString(), out updated) ? updated : 0;
            Inp72DetailOpt.UpdatedAt = AppData.ClassSatoo.GetVdate(updated).ToString();
            Inp72DetailOpt.ManualInpNo = item.ManualInpNo;
            Inp72DetailOpt.OrderDate = DateTime.ParseExact(item.InvCountDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            Inp72DetailOpt.DeliverDate = DateTime.ParseExact(item.DeliverDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            Inp72DetailOpt.TranCate = item.TranCate;
            Inp72DetailOpt.InpCd = new BtListHelper(item.InpStaffCD, item.StaffName);
            Inp72DetailOpt.RecDestCd = new BtListHelper(item.ClientCD2, item.CustomerName);
            Inp72DetailOpt.SupplierCd = new BtListHelper(item.ClientCD1, item.SupplierName);
            Text10 = item.MarkupRate1 ?? 0;
            Text11 = item.TaxableAmount ?? 0;
            Inp72DetailOpt.Num = item.TotalNum;
            Text13 = item.TotalDetailAmt ?? 0;
            Text14 = item.TaxIncluded ?? 0;
            Text15 = item.TaxExcluded ?? 0;
            Inp72DetailOpt.RetailPrice = item.TotalRetail ?? 0;
            Inp72DetailOpt.WholesalePrice = item.TotalWholesale ?? 0;
            Inp72DetailOpt.Memo = item.Memo;
            Text19 = item.AccFlg ?? 0;
            Text20 = item.SlipCate ?? 0;
            Text21 = item.ModSeq ?? 0;
            Text22 = item.StoreCD;
            Inp72DetailOpt.RelatedNo = item.RelatedSlipNo.ToString();
            Inp72DetailOpt.RelatedNo2 = item.RelatedSlipNo2.ToString();
            Text26 = item.MarkupRate ?? 0;
            Text27 = item.MarkupRate2 ?? 0;
            Text28 = item.TaxCD ?? 0;
            Text29 = item.TaxCalcMethod ?? 0;
            Text30 = item.TaxRound ?? 0;
            SelectedTabIndex = 1;

            /* 明細取得(F5) */
            /* 伝票NOが無い場合 */
            if (string.IsNullOrEmpty(Inp72DetailOpt.SlipNo)) return;
            BizArray wrk_para = new BizArray();
            wrk_para[0] = Inp72DetailOpt.SlipNo;
            wrk_para[1] = Inp72DetailOpt.TranCate;
            var ret_csv = OnQueryDetail(wrk_para);
            var list = (from DataRow dr in ret_csv.Rows
                        select new Inp72TranDetail
                        {
                            SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDouble(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDouble(dr["VDATE_UPDATE"]),
                            TranCate = Convert.ToInt16(dr["明細取引区分"]),
                            ProdCD = dr["商品CD"].ToString() ?? string.Empty,
                            ColorCD = dr["色CD"].ToString() ?? string.Empty,
                            SizeCD = dr["サイズCD"].ToString() ?? string.Empty,
                            DetailName = dr["明細名称"].ToString() ?? string.Empty,
                            Num = Convert.ToInt32(dr["数量"]),
                            UnitPrice = Convert.ToInt64(dr["単価"]),
                            TotalAmount = Convert.ToInt64(dr["金額"]),
                            TaxIncluded = Convert.ToInt64(dr["内税消費税"]),
                            TaxExcluded = Convert.ToInt64(dr["外税消費税"]),
                            RetailUnit = Convert.ToInt64(dr["上代単価"]),
                            RetailAmount = Convert.ToInt64(dr["上代金額"]),
                            WholesalesUnit = Convert.ToInt64(dr["下代単価"]),
                            WholesalesAmount = Convert.ToInt64(dr["下代金額"]),
                            DetailMemo = dr["明細メモ"].ToString() ?? string.Empty,
                            TaxCalcMethod = Convert.ToInt16(dr["消費税計算方法"]),
                            ProdSerial = dr["商品シリアル"].ToString() ?? string.Empty,
                            RelatedSlipNo = Convert.ToInt64(dr["関連伝票NO"]),
                            RelatedSlipLineNo = Convert.ToInt64(dr["関連伝票行NO"]),
                            Jancode = dr["JANCODE"].ToString() ?? string.Empty,
                            CostFlg = Convert.ToInt16(dr["原価FLG"]),
                            CompleteFlg = Convert.ToInt16(dr["完了FLG"]),
                            ColorName = dr["COL名"].ToString() ?? string.Empty,
                            SizeName = dr["サイズ名"].ToString() ?? string.Empty,
                            MrkProdNum = dr["MKR品番"].ToString() ?? string.Empty
                        }).OrderBy(x => x.SeqNo);
            Inp72TranDetails = new ObservableCollection<Inp72TranDetail>(list);

            int? kei = 0; /* 数量 */
            long? koukei = 0; /* 上代金額 */
            long? koukei1 = 0;/* 下代金額 */

            kei = list.Sum(x => x.Num);
            koukei = list.Sum(x => x.RetailAmount);
            koukei1 = list.Sum(x => x.WholesalesAmount);

            Inp72DetailOpt.Num = kei;
            Inp72DetailOpt.RetailPrice = koukei;
            Inp72DetailOpt.WholesalePrice = koukei1;
            Mess2 = "明細データ取得しました";
            Chg_flg = 1;
        }
        /// <summary>
        /// 削除(F7)
        /// </summary>
        [RelayCommand]
        private void DoDelete()
        {
            if (Inp72TranHeadItem == null) return;
            DateTime sir_day = DateTime.ParseExact(Inp72TranHeadItem.InvCountDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                Mess2 = "修正可能な伝票ではありません。";
                return;
            }
            var messbox = System.Windows.MessageBox.Show("削除しますか?", "確認", System.Windows.MessageBoxButton.YesNo);
            if (messbox == System.Windows.MessageBoxResult.Yes)
            {
                BizArray col_wrk = new BizArray();
                BizArray col_wrk2 = new BizArray();
                col_wrk[0] = "MOD_SEQ";
                col_wrk2[0] = "0";
                var ret = AppData.Http!.AspxSqlExe(DBDef.DB_DML.DELETE, "TRAN_VTORI0",
                                                       long.Parse(Inp72TranHeadItem.SeqNo.ToString()),
                                                       Inp72TranHeadItem.VdateUpdate.ToString(),
                                                       col_wrk.ToArray(), col_wrk2.ToArray());
                if (ret.Code == 0)
                {
                    Mess2 = "データ削除しました";
                    Inp72TranHeads.Remove(Inp72TranHeadItem);
                }
                else if (ret.Code == -1)
                    Mess2 = "ロックエラーです";
                else if (ret.Code == -2)
                    Mess2 = "他で更新されていますので、登録されていません";
                else
                    Mess2 = "データ削除できませんでした";
            }
        }
        #endregion

        #region Functions
        /* パラメータ生成処理 */
        private void CreatePara()
        {
            if (Inp72SearchOpt == null) return;
            param1 = new BizArray();
            param2 = new BizArray();

            param1[0] = Inp72SearchOpt.SlipStartNo;
            param1[1] = Inp72SearchOpt.SlipEndNo;
            param1[2] = Inp72SearchOpt.DeliverStartDate?.ToString("yyyyMMdd");
            param1[3] = Inp72SearchOpt.DeliverEndDate?.ToString("yyyyMMdd");
            param1[4] = Inp72SearchOpt.TranStartCat;
            param1[5] = Inp72SearchOpt.TranEndCat;
            param1[6] = Inp72SearchOpt.RelatedStartNo;
            param1[7] = Inp72SearchOpt.RelatedEndNo;
            param1[8] = Inp72SearchOpt.RelatedStartNo2;
            param1[9] = Inp72SearchOpt.RelatedEndNo2;
            param1[10] = Inp72SearchOpt.SupplierStartCd.Code;
            param1[11] = Inp72SearchOpt.SupplierEndCd.Code;
            param1[12] = Inp72SearchOpt.RecDestStartCd.Code;
            param1[13] = Inp72SearchOpt.RecDestEndCd.Code;
            param1[14] = Inp72SearchOpt.DeliverStartDate?.ToString("yyyyMMdd");
            param1[15] = Inp72SearchOpt.DeliverEndDate?.ToString("yyyyMMdd");
            param1[16] = Inp72SearchOpt.SlipStartNo;
            param1[17] = Inp72SearchOpt.ManualInpStartNo;
            param1[18] = Inp72SearchOpt.ManualInpEndNo;
            param1[19] = Inp72SearchOpt.InpStartCd.Code;
            param1[20] = Inp72SearchOpt.InpEndCd.Code;
            if (PrdToggle == 1)
            {
                param2[0] = Inp72SearchOpt.ProdStartCd.Code;
                param2[1] = Inp72SearchOpt.ProdEndCd.Code;
            }
        }
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

            /* 05.03.20 F-T変更 */
            string sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            for (var i = 0; i < col_list.Count; i++)
            {
                sql_query += "A." + col_list[i] + ",";
            }
            sql_query += "B.名前 担当名,C.仕入先名 仕入先名,D.得意先名 得意先名,C.掛率,C.掛率2,C.消費税CD,C.消費税計算方法,C.消費税端数 ";
            sql_query += " from HC$Tran_VTORI0 A,HC$MASTER_SHAIN B,HC$Master_SIIRE C,HC$MASTER_TOKUI D ";
            sql_query += "where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.取引先CD2=D.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A.納品日 between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.取引先CD2 between :13 and :14 ";
            sql_query += "and  A.納品日 between :15 and :16 ";
            sql_query += "and A.伝票処理区分=3 and A.SEQ_NO>=:17 ";

            /* 2016.10.18 検索項目追加 */
            sql_query += "and A.手入力伝票NO between :18 and :19 ";
            sql_query += "and A.入力社員CD between :20 and :21 ";

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
                    + ") A WHERE EXISTS (SELECT /*+ INDEX(E HC$_NK_TORI23) */ 'X' FROM HC$Tran_VTORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (v_para.Count - 1).ToString() + " AND :" + (v_para.Count).ToString() + ")";
            }
            sql_query += " ORDER BY A.SEQ_NO " + v_sort;
            sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);
            ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());

            var list = (from DataRow dr in ret_csv.Rows
                        select new Inp72TranHead
                        {
                            SeqNo = Convert.ToInt32(dr["SEQ_NO"]),
                            VdateCreate = Convert.ToDouble(dr["VDATE_CREATE"]),
                            VdateUpdate = Convert.ToDouble(dr["VDATE_UPDATE"]),
                            ManualInpNo = dr["手入力伝票NO"].ToString() ?? string.Empty,
                            InvCountDate = dr["在庫計上日"].ToString() ?? string.Empty,
                            DeliverDate = dr["納品日"].ToString() ?? string.Empty,
                            TranCate = dr["取引区分"].ToString() ?? string.Empty,
                            InpStaffCD = dr["入力社員CD"].ToString() ?? string.Empty,
                            ClientCD2 = dr["取引先CD2"].ToString() ?? string.Empty,
                            ClientCD1 = dr["取引先CD1"].ToString() ?? string.Empty,
                            MarkupRate1 = int.TryParse(dr["掛率1"].ToString(), out var mark_rate1) ? mark_rate1 : 0,
                            TaxableAmount = Convert.ToInt64(dr["外税対象金額"]),
                            TotalNum = int.TryParse(dr["数量合計"].ToString(), out var total_num) ? total_num : 0,
                            TotalDetailAmt = Convert.ToInt64(dr["明細金額合計"]),
                            TaxIncluded = Convert.ToInt64(dr["内税消費税"]),
                            TaxExcluded = Convert.ToInt64(dr["外税消費税"]),
                            TotalRetail = Convert.ToInt64(dr["上代合計"]),
                            TotalWholesale = Convert.ToInt64(dr["下代合計"]),
                            Memo = dr["メモ"].ToString() ?? string.Empty,
                            AccFlg = int.TryParse(dr["掛計上FLG"].ToString(), out var acc_flg) ? acc_flg : 0,
                            SlipCate = int.TryParse(dr["伝票処理区分"].ToString(), out var slip_cate) ? slip_cate : 0,
                            ModSeq = Convert.ToInt64(dr["MOD_SEQ"]),
                            StoreCD = dr["倉庫CD"].ToString() ?? string.Empty,
                            RelatedSlipNo = Convert.ToInt64(dr["関連伝票NO"]),
                            RelatedSlipNo2 = Convert.ToInt64(dr["関連伝票NO2"]),
                            StaffName = dr["担当名"].ToString() ?? string.Empty,
                            SupplierName = dr["仕入先名"].ToString() ?? string.Empty,
                            CustomerName = dr["得意先名"].ToString() ?? string.Empty,
                            MarkupRate = int.TryParse(dr["掛率"].ToString(), out var mark_rate) ? mark_rate : 0,
                            MarkupRate2 = int.TryParse(dr["掛率2"].ToString(), out var mark_rate2) ? mark_rate2 : 0,
                            TaxCD = Convert.ToInt64(dr["消費税CD"]),
                            TaxCalcMethod = int.TryParse(dr["消費税計算方法"].ToString(), out var tax_calc) ? tax_calc : 0,
                            TaxRound = int.TryParse(dr["消費税端数"].ToString(), out var tax_round) ? tax_round : 0
                        }).OrderByDescending(c => c.SeqNo);
            Inp72TranHeads = new ObservableCollection<Inp72TranHead>(list);
        }
        private DataTable OnQueryDetail(BizArray v_para)
        {
            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE,";
            for (var i = 0; i < col_list2.Count; i++)
            {
                sql_query += "A." + col_list2[i] + ",";
            }
            sql_query += " NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=A.色CD),'.') COL名";
            sql_query += ",GET_SIZENAME(A.商品CD,A.サイズCD) サイズ名";
            sql_query += ",NVL((select H.メーカー品番 from HC$master_shohin H where H.商品CD=A.商品CD),'.') MKR品番";
            sql_query += " from HC$tran_vtori1 A";
            sql_query += " where ヘッダNO=:1 and 明細取引区分=:2 order by ヘッダNO,行NO";

            var ret_data = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
            return ret_data;
        }
        private string OnQueryPrint(BizArray wrk_para1, BizArray v_sho = null)
        {
            var wrk_para = new BizArray();
            for (var i = 0; i < wrk_para1.Count; i++)
            {
                wrk_para[i] = wrk_para1[i];
            }

            /* 05.03.20 F-T変更 */
            var sql_query = "select A.SEQ_NO";      /* 伝票NO */
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";         /* データを日付として表示させるための処理 */
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時,";
            sql_query += "'発注伝票一覧',";		/* Pssタイトル用 */

            for (var i = 0; i < col_list.Count; i++)
            {
                sql_query += "A." + col_list[i] + ",";
            }

            sql_query += " B.名前 担当名,C.仕入先名 仕入先名,D.得意先名 得意先名 ";
            sql_query += ", A.SYSFLG";
            /* 画面上ComboBoxの文字列取得 */
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("商品発注区分", "A.取引区分") + " 取引区分名";		/* 関数Combo処理 */
            sql_query += " from HC$Tran_VTORI0 A,HC$MASTER_SHAIN B,HC$Master_SIIRE C,HC$MASTER_TOKUI D ";
            sql_query += "where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.取引先CD2=D.得意先CD(+)) ";
            sql_query += "and A.SEQ_NO between :1 and :2 and A.納品日 between :3 and :4 and A.取引区分 between :5 and :6 ";
            sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.取引先CD2 between :13 and :14 ";
            sql_query += "and  A.納品日 between :15 and :16 ";
            sql_query += "and A.伝票処理区分=3 and A.SEQ_NO>=:17";

            /* 2016.10.18 検索項目追加 */
            sql_query += "and A.手入力伝票NO between :18 and :19 ";
            sql_query += "and A.入力社員CD between :20 and :21 ";

            if (PrdToggle == 1 && v_sho != null)
            {
                wrk_para[wrk_para.Count] = v_sho[0];
                wrk_para[wrk_para.Count] = v_sho[1];
                sql_query = "SELECT A.* FROM (" + sql_query
                + ") A WHERE EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$Tran_VTORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (wrk_para.Count - 1) + " AND :" + (wrk_para.Count) + ")";
            }
            sql_query += " order by A.SEQ_NO";

            if (CSV_flg == 1)
            {
                sql_query = ""
                            + "select "
                            + "a.SEQ_NO 伝票No , "
                            + " a.在庫計上日 発注日 , "
                            + " a.納品日 , "
                            + " a.伝票処理区分 伝票区分 , "
                            + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ') - 1) 取引区分CD , "
                            + " substrb(a.取引区分名, instr(a.取引区分名, ' ') + 1) 取引区分名 , "
                            + " a.掛率1 掛率 , "
                            + " a.SYSFLG , "
                            + " a.入力社員CD 入力者CD , "
                            + " a.担当名 入力者名 , "
                            + " a.数量合計 数量計 , "
                            + " a.明細金額合計 金額計 , "
                            + " a.上代合計 , "
                            + " a.取引先CD1 仕入先CD , "
                            + " a.仕入先名 , "
                            + " a.取引先CD2 倉庫CD , "
                            + " a.得意先名 倉庫名, "
                            + " a.手入力伝票NO 手入力No , "
                            + " a.関連伝票NO 関連No1 , "
                            + " a.関連伝票NO2 関連No2 , "
                            + " a.外税消費税 消費税計 , "
                            + " a.下代合計 , "
                            + " a.メモ "
                            + "  from "
                            + "(" + sql_query + ") a";
            }
            return AppData.Http!.AspxSqlQueryCsv(sql_query, wrk_para.ToArray(), "cvnet72prn_header.qfm");
        }
        /*********************/
        /*明細印刷BTN用検索処理 */
        /*********************/
        private string OnQueryDetailPrint(BizArray wrk_para1, int hdFlg, BizArray v_sho = null)
        {
            var wrk_para = new BizArray();
            for (var i = 0; i < wrk_para1.Count; i++)
            {
                wrk_para[i] = wrk_para1[i];
            }

            /* ヘッダ情報 */
            var sql_query = "select A.SEQ_NO";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_CREATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_CREATE),10,6) 作成日時";
            sql_query += ",SUBSTR(GET_VDATE(a.VDATE_UPDATE),0,8)||SUBSTR(GET_VDATE(a.VDATE_UPDATE),10,6) 更新日時,";
            sql_query += "'発注伝票明細',";		/* Pssタイトル用 */

            for (var i = 0; i < col_list.Count; i++)
            {
                sql_query += "A." + col_list[i] + ",";
            }

            sql_query += " B.名前 担当名,C.仕入先名 仕入先名,D.得意先名 得意先名 ";
            sql_query += ", A.SYSFLG";
            sql_query += "," + AppData.ClassCvnet.comboItem00.GetCaseStr("商品発注区分", "A.取引区分") + " 取引区分名,";

            /* 明細情報 */
            for (var i = 0; i < col_list2.Count; i++)
            {
                sql_query += "T1." + col_list2[i] + " tt" + i + ",";
            }

            sql_query += " NVL((select H.名称 from HC$master_meisho H where H.名称区分='COL' and H.名称CD=T1.色CD),'.') COL名";
            sql_query += ",GET_SIZENAME(T1.商品CD,T1.サイズCD) サイズ名";
            sql_query += ",T1.行NO";
            sql_query += ",decode(T1.完了FLG,1,'完了','未完') 完了FLG名";    /* 完了FLG追加 2008.02.05 */
            sql_query += ",NVL((select H.メーカー品番 from HC$master_shohin H where H.商品CD=t1.商品CD),'.') MKR品番";

            sql_query += " from HC$Tran_VTORI0 A, HC$MASTER_SHAIN B, HC$Master_SIIRE C, HC$MASTER_TOKUI D ";
            sql_query += " ,HC$tran_VTORI1 T1";
            sql_query += " where (A.入力社員CD=B.社員CD(+)) and (A.取引先CD1=C.仕入先CD(+)) and (A.取引先CD2=D.得意先CD(+)) ";
            sql_query += "and (A.SEQ_NO=T1.ヘッダNO) ";
            if (hdFlg == 1)
            {
                /* 検索画面「明細印刷」 */
                sql_query += "and A.SEQ_NO between :1 and :2 and A.納品日 between :3 and :4 and A.取引区分 between :5 and :6 ";
                sql_query += "and A.関連伝票NO between :7 and :8 and A.関連伝票NO2 between :9 and :10 and A.取引先CD1 between :11 and :12 and A.取引先CD2 between :13 and :14 ";
                sql_query += "and  A.納品日 between :15 and :16 ";
                sql_query += "and A.伝票処理区分=3 and A.SEQ_NO>=:17";

                /* 2016.10.18 検索項目追加 */
                sql_query += "and A.手入力伝票NO between :18 and :19 ";
                sql_query += "and A.入力社員CD between :20 and :21 ";

                if (PrdToggle == 1 && v_sho != null)
                {
                    wrk_para[wrk_para.Count] = v_sho[0];
                    wrk_para[wrk_para.Count] = v_sho[1];
                    sql_query += " AND EXISTS (SELECT /*+ INDEX(E 	HC$_NK_TORI23) */ 'X' FROM HC$Tran_VTORI1 E WHERE A.SEQ_NO=E.ヘッダNO AND E.商品CD BETWEEN :" + (wrk_para.Count - 1) + " AND :" + (wrk_para.Count) + ")";
                }
            }
            else
            {
                /* 修正・登録画面「印刷」 */
                sql_query += " and T1.ヘッダNO=:1 and T1.明細取引区分=:2 ";
            }
            sql_query += " order by T1.ヘッダNO, T1.行NO";

            /* 2016.10.13 CSV出力用 */
            if (CSV_flg == 1)
            {
                sql_query = ""
                + "select "
                    + " a.SEQ_NO 伝票No , "
                    + " a.在庫計上日 発注日 , "
                    + " a.納品日 , "
                    + " a.伝票処理区分 伝票区分 , "
                    + " substrb(a.取引区分名, 1, instr(a.取引区分名, ' ')-1) 取引区分CD , "
                    + " substrb(a.取引区分名, instr(a.取引区分名, ' ')+1) 取引区分名 , "
                    + " a.掛率1 掛率 , "
                    + " a.SYSFLG , "
                    + " a.入力社員CD 入力者CD , "
                    + " a.担当名 入力者名 , "
                    + " a.数量合計 数量計 , "
                    + " a.明細金額合計 金額計 , "
                    + " a.上代合計, "
                    + " a.下代合計 , "
                    + " a.取引先CD1 仕入先CD , "
                    + " a.仕入先名 , "
                    + " a.取引先CD2 倉庫CD , "
                    + " a.得意先名 倉庫名, "
                    + " a.手入力伝票NO 手入力No , "
                    + " a.関連伝票NO 関連No1 ,"
                    + " a.関連伝票NO2 関連No2 , "
                    + " a.外税消費税 消費税計 , "
                    + " a.メモ , "
                    + " a.行NO 行No , "
                    + " a.TT1 商品CD , "
                    + " a.TT4 商品名 , "
                    + " a.TT2 色CD , "
                    + " a.COL名 色名 , "
                    + " a.TT3 サイズCD , "
                    + " a.サイズ名 サイズ名 , "
                    + " a.TT21 完了FLG , "
                    + " a.完了FLG名 完了 , "
                    + " a.TT5 数量 , "
                    + " a.TT6 単価 , "
                    + " a.TT10 上代単価 , "
                    + " a.TT12 下代単価 , "
                    + " a.TT20 原価FLG , "
                    + " a.TT14 摘要 ,  "
                    + " a.TT9 消費税 , "
                    + " a.TT7 金額 , "
                    + " a.TT11 上代金額 , "
                    + " a.TT13 下代金額 , "
                    /* 2021.04.27 追加 */
                    + " nvl(s.上代,0) マスタ上代 , "
                    + " nvl(s.元上代,0) 元上代 , "
                    + " nvl(s.仕入価格,0) 仕入価格 , "
                    + " GET_GENKA(a.TT1,0,a.在庫計上日,a.TT2,a.TT3) マスタ原価 , "
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
                    + " (" + sql_query + ") a , "
                    + " HC$MASTER_SHOHIN s , "
                    + " HC$MASTER_SHOHIN_JAN j "
                + " where "
                    + " a.TT1=s.商品CD(+) "
                    + " and a.TT1=j.商品CD(+) "
                    + " and a.TT2=j.色CD(+) "
                    + " and a.TT3=j.サイズCD(+) "
                + " order by "
                    + " a.SEQ_NO , "
                    + " a.行NO "
                + "";
            }
            return AppData.Http!.AspxSqlQueryCsv(sql_query, wrk_para.ToArray(), "cvnet72prn_detail.qfm");
        }
        #endregion
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        #region Variables  
        [ObservableProperty]
        public Inp72DetailOpt? m_Inp72DetailOpt;

        [ObservableProperty]
        private int m_Inp72DetailSelIdx = 0;

        [ObservableProperty]
        public ObservableCollection<Inp72TranDetail> m_Inp72TranDetails;
        #endregion

        #region Text Variable
        private int Text10 = 0; /* 伝票上の掛率 */
        private int Text19 = 0; /* 掛計上FLG */
        private int Text26 = 0; /* 仕入先、掛率 */
        private int Text27 = 0; /* 仕入先、掛率2 */
        private long Text28 = 0; /* 仕入先、消費税CD */
        private int Text29 = 0; /* 仕入先、消費税計算方法 */
        private int Text30 = 0; /* 仕入先、消費税端数 */
        private string Text22 = string.Empty; /* 倉庫CD */

        /* 非表示項目 */
        private long Text11 = 0;
        private long Text13 = 0;
        private long Text14 = 0;
        private long Text15 = 0;
        private int Text20 = 0;
        private long Text21 = 0;

        private BizArray err_pos = new BizArray();
        #endregion

        #region ComboList Variable 
        [ObservableProperty]
        public Dictionary<int, string> m_StatusList;
        #endregion

        #region Dialog Search
        [RelayCommand]
        private void SelSupplierCD(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72DetailOpt != null)
            {
                Inp72DetailOpt.SupplierCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelRecDestCD(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72DetailOpt != null)
            {
                Inp72DetailOpt.RecDestCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelInpCD(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72DetailOpt != null)
            {
                Inp72DetailOpt.InpCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        [RelayCommand]
        private void SelProdCD(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && Inp72DetailOpt != null)
            {
                Inp72DetailOpt.ProdCd = new BtListHelper(get_sel00.Code, get_sel00.Name);
            }
        }
        #endregion

        #region Events
        /// <summary>
        /// ｵｰﾙｸﾘｱ
        /// </summary>
        [RelayCommand]
        private void DoAllClear()
        {
            if (Inp72TranDetails.Count == 0) return;
            var messbox = System.Windows.MessageBox.Show("明細表示が全てクリアされます。よろしいですか？", "確認", System.Windows.MessageBoxButton.OKCancel);
            if (messbox == System.Windows.MessageBoxResult.OK)
            {
                Inp72DetailOpt = new Inp72DetailOpt();
                Inp72DetailOpt.TranCate = TranCatList?.Where(x => x.Key == "10").FirstOrDefault().Key ?? "10";

                Text10 = 0;
                Text19 = 0;
                Text26 = 0;
                Text27 = 0;
                Text28 = 0;
                Text29 = 0;
                Text30 = 0;

                /* 非表示項目 */
                Text11 = 0;
                Text13 = 0;
                Text14 = 0;
                Text15 = 0;
                Text20 = 13;
                Text21 = 0;
            }
        }
        /// <summary>
        /// 展開商品CD
        /// </summary>
        [RelayCommand]
        private void DoAvailProd()
        {
            if (Inp72DetailOpt == null || Inp72DetailOpt.ProdCd == null) return;
            if (string.IsNullOrEmpty(Inp72DetailOpt.ProdCd.Code))
            {
                Mess2 = "先に得意先を入力してください";
                return;
            }
            if (OnCheckMst() < 0) return;

            var v_para = new BizArray();
            v_para[0] = Inp72DetailOpt.ProdCd.Code;

            var v_para2 = new BizArray();
            v_para2[0] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
            v_para2[1] = Inp72DetailOpt.RecDestCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) v_para2[0] = "19000101";

            var vm_result = AppData.DlgService.GetSku01(v_para.ToArray(), v_para2.ToArray());
            if (vm_result != null && vm_result.ret_para != null)
            {
                OnSetMultiRow(vm_result.ret_para);
            }
        }
        /// <summary>
        /// 一括Refresh⇔
        /// </summary>
        [RelayCommand]
        private void DoRefreshAll()
        {
            var messbox = System.Windows.MessageBox.Show("全ての明細行をRefreshします", "確認", System.Windows.MessageBoxButton.OKCancel);
            if (messbox == System.Windows.MessageBoxResult.OK)
            {
                foreach (var row in Inp72TranDetails)
                {
                    DoRefresh(row);
                }
                Mess2 = "Refreshしました";
            }
        }
        /// <summary>
        /// ﾊﾞｰｺｰﾄﾞ入力
        /// </summary>
        [RelayCommand]
        private void InpBarcode()
        {
            if (string.IsNullOrEmpty(Inp72DetailOpt.RecDestCd.Code)) return;

            var wrk_para = new BizArray();
            wrk_para[0] = Text20.ToString();
            wrk_para[1] = Inp72DetailOpt.RecDestCd.Code;
            wrk_para[2] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
            /* ジャコモ用取引区分追加 */
            wrk_para[5] = Inp72DetailOpt.TranCate;

            var vm_result = AppData.DlgService.GetBcd01(wrk_para.ToArray());
            if (vm_result != null && vm_result.ret_para != null)
            {
                OnSetMultiRow(vm_result.ret_para);
            }
        }
        /// <summary>
        /// 商品CD検索
        /// </summary>
        [RelayCommand]
        private void DoSearchProd()
        {
            var vm = new SubDlgSyoKenSakuViewModel();
            var window = new SubDlgSyoKenSakuView { DataContext = vm };
            window.ShowDialog();
            var result = vm.Result;
            if (result != null)
            {
                foreach (var row in Inp72TranDetails)
                {

                    if (row.ProdCD == result[0])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor1"];
                        row.ProdBgColor = brush.Color.ToString();
                    }
                    if (row.ProdCD == result[1])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor2"];
                        row.ProdBgColor = brush.Color.ToString();
                    }
                    if (row.ProdCD == result[2])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor3"];
                        row.ProdBgColor = brush.Color.ToString();
                    }
                    if (row.ProdCD == result[3])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor4"];
                        row.ProdBgColor = brush.Color.ToString();
                    }
                    if (row.ProdCD == result[4])
                    {
                        var brush = (SolidColorBrush)Application.Current.Resources["SearchColor5"];
                        row.ProdBgColor = brush.Color.ToString();
                    }
                }
            }
        }
        /// <summary>
        /// 修正(F6)
        /// </summary>
        [RelayCommand]
        private void DoUpdate()
        {
            /* 入力エラーチェック */
            if (OnCheckError() < 0) return;
            if (OnCheckMst() < 0) return;

            /* 修正可能or不可能チェックに伝票NO追加 */
            if (Inp72DetailOpt == null || string.IsNullOrEmpty(Inp72DetailOpt.SlipNo)) return;
            if (Inp72TranHeadItem == null) return;
            var sir_day = DateTime.ParseExact(Inp72TranHeadItem.InvCountDate, "yyyyMMdd", CultureInfo.InvariantCulture);
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                Mess2 = "修正可能な伝票ではありません。";
                return;
            }
            sir_day = Inp72DetailOpt.OrderDate ?? DateTime.Now;
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                Mess2 = "修正可能な日付ではありません。";
                return;
            }
            /* 明細色クリア */
            OnCellStdColor();
            long now_mod_seq = long.TryParse(Inp72TranHeadItem.SeqNo.ToString(), out long mod_seq) ? mod_seq : 0;
            string? now_mod_vdate = Inp72TranHeadItem.VdateUpdate.ToString() ?? string.Empty;
            var col01 = new BizArray();
            var col02 = new BizArray();
            col01[0] = "MOD_SEQ";
            col02[0] = "10"; 
            var ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.LOCK, "TRAN_VTORI0", now_mod_seq, now_mod_vdate, col01.ToArray(), col02.ToArray());
            if (ret_val.Code == -1)
            {
                Mess2 = "ロックエラーです";
                return;
            }
            else if (ret_val.Code == -2)
            {
                Mess2 = "他で更新されていますので、登録されていません";
                return;
            }
            else if (ret_val.Code < 0)
            {
                Mess2 = "ロックエラー !";
                return;
            }
            long new_seq = ret_val.NewSeq;
            DateTime v_start = DateTime.Now;
            /* 明細チェック用 */
            var err_wrk = new BizArray();
            /* 各明細の登録処理 */
            Text20 = 3;
            int ret_detail = AspxParaGetDetail(Inp72TranDetails, col_list2, new_seq, Text20,
                                               Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd"), err_wrk);
            /* テスト：2004/11/10 140明細=7sec */
            if (ret_detail < 0)
            {
                AppData.Http!.AspxSqlExe(DBDef.DB_DML.UNLOCK, "TRAN_VTORI0", now_mod_seq, now_mod_vdate, null, null);
                Mess2 = "明細登録エラー ! 赤い項目の[一覧]から選択して下さい｡";
                //OnErrCells(err_wrk);
                return;
            }
            Text21 = new_seq;
            if (string.IsNullOrEmpty(Inp72DetailOpt.InpCd.Code))
                Inp72DetailOpt.InpCd = new BtListHelper(AppData.ClassSatoo.SHAIN_CD, AppData.ClassSatoo.SHAIN_Name);

            //AspxParaGet
            col02[0] = Inp72DetailOpt.ManualInpNo; // 手入力伝票NO
            col02[1] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd"); //在庫計上日
            col02[2] = Inp72DetailOpt.DeliverDate?.ToString("yyyyMMdd"); //納品日
            col02[3] = Inp72DetailOpt.TranCate; //取引区分
            col02[4] = Inp72DetailOpt.InpCd.Code; //入力社員CD
            col02[5] = Inp72DetailOpt.RecDestCd.Code; //取引先CD2
            col02[6] = Inp72DetailOpt.SupplierCd.Code; //取引先CD1
            col02[7] = Text10.ToString(); //掛率1
            col02[8] = Text11.ToString(); //外税対象金額
            col02[9] = Inp72DetailOpt.Num.ToString(); //数量合計
            col02[10] = Text13.ToString(); // 明細金額合計
            col02[11] = Text14.ToString(); // 内税消費税
            col02[12] = Text15.ToString(); // 外税消費税
            col02[13] = Inp72DetailOpt.RetailPrice.ToString(); // 上代合計
            col02[14] = Inp72DetailOpt.WholesalePrice.ToString(); // 下代合計
            col02[15] = Inp72DetailOpt.Memo; // メモ
            col02[16] = Text19.ToString(); // 掛計上FLG
            col02[17] = Text20.ToString(); // 伝票処理区分
            col02[18] = Text21.ToString(); // MOD_SEQ
            col02[19] = Text22.ToString(); // 倉庫CD
            col02[20] = Inp72DetailOpt.RelatedNo; // 関連伝票NO
            col02[21] = Inp72DetailOpt.RelatedNo2; // 関連伝票NO2
            /* 2022.02.09 倉庫CDは倉庫CDに設定 */
            col02[19] = Inp72DetailOpt.RecDestCd.Code;
            ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UPDATE, "TRAN_VTORI0", now_mod_seq, now_mod_vdate,
                                               col_list.ToArray(), col02.ToArray());
            if (ret_val.Code == 0)
            {
                if (Inp72TranHeadItem != null)
                {
                    int idx = Inp72TranHeads.IndexOf(Inp72TranHeadItem);
                    Inp72TranHeads[idx].VdateUpdate = double.TryParse(ret_val.VDate.ToString(), out var _update) ? _update : 0;
                    double updatedAt = double.TryParse(ret_val.VDate, out var _updatedAt) ? _updatedAt : 0;
                    Inp72DetailOpt.UpdatedAt = AppData.ClassSatoo.GetVdate(updatedAt).ToString();

                    Inp72TranHeads[idx].ManualInpNo = col02[0];
                    Inp72TranHeads[idx].InvCountDate = col02[1];
                    Inp72TranHeads[idx].DeliverDate = col02[2];
                    Inp72TranHeads[idx].TranCate = col02[3];
                    Inp72TranHeads[idx].InpStaffCD = col02[4];
                    Inp72TranHeads[idx].ClientCD2 = col02[5];
                    Inp72TranHeads[idx].ClientCD1 = col02[6];
                    Inp72TranHeads[idx].MarkupRate1 = int.TryParse(col02[7], out int mark_rate1) ? mark_rate1 : 0;
                    Inp72TranHeads[idx].TaxableAmount = long.TryParse(col02[8], out long tax_amt) ? tax_amt : 0;
                    Inp72TranHeads[idx].TotalNum = int.TryParse(col02[9], out int total_num) ? total_num : 0;
                    Inp72TranHeads[idx].TotalDetailAmt = long.TryParse(col02[10], out long detail_amt) ? detail_amt : 0;
                    Inp72TranHeads[idx].TaxIncluded = long.TryParse(col02[11], out long tax_include) ? tax_include : 0;
                    Inp72TranHeads[idx].TaxExcluded = long.TryParse(col02[12], out long tax_exclude) ? tax_exclude : 0;
                    Inp72TranHeads[idx].TotalRetail = long.TryParse(col02[13], out long retail) ? retail : 0;
                    Inp72TranHeads[idx].TotalWholesale = long.TryParse(col02[14], out long wholesale) ? wholesale : 0;
                    Inp72TranHeads[idx].Memo = col02[15];
                    Inp72TranHeads[idx].AccFlg = int.TryParse(col02[16], out int acc_flg) ? acc_flg : 0;
                    Inp72TranHeads[idx].SlipCate = int.TryParse(col02[17],out int slip_cate) ? slip_cate : 0;
                    Inp72TranHeads[idx].ModSeq = long.TryParse(col02[18], out long moq_seq) ? moq_seq : 0;
                    Inp72TranHeads[idx].StoreCD = col02[19];
                    Inp72TranHeads[idx].RelatedSlipNo = long.TryParse(col02[20], out long slip_no) ? slip_no : 0;
                    Inp72TranHeads[idx].RelatedSlipNo2 = long.TryParse(col02[21], out long slip_no2) ? slip_no2 : 0;

                    Inp72TranHeads[idx].StaffName = Inp72DetailOpt.InpCd.Name;
                    Inp72TranHeads[idx].SupplierName = Inp72DetailOpt.SupplierCd.Name;
                    Inp72TranHeads[idx].CustomerName = Inp72DetailOpt.RecDestCd.Name;

                    SelectedTabIndex = 0;
                    System.Windows.MessageBox.Show("データ修正しました (" + AppData.ClassSatoo.GetDateDiff(v_start, DateTime.Now) + ")",
                                                   "確認", System.Windows.MessageBoxButton.OK);
                }
                else if (ret_val.Code == -1) Mess2 = "ロックエラーです";
                else if (ret_val.Code == -2) Mess2 = "他で更新されていますので、登録されていません";
                else
                {
                    ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.UNLOCK, "TRAN_VTORI0", now_mod_seq, now_mod_vdate, null, null);
                    Mess2 = "データ修正できませんでした";
                }
            }
        }
        /// <summary>
        /// 追加(F8)
        /// </summary>
        [RelayCommand]
        private void DoAdd()
        {
            /* 入力エラーチェック */
            if (OnCheckError() < 0) return;
            if (OnCheckMst() < 0) return;
            var sir_day = Inp72DetailOpt.OrderDate ?? DateTime.Now;
            if (AppData.ClassCvnet.CheckImpDate(sir_day) < 0)
            {
                Mess2 = "登録可能な日付ではありません。";
                return;
            }
            /* 明細色クリア */
            OnCellStdColor();
            var col01 = new BizArray();
            var col02 = new BizArray();
            col01[0] = "SESS_ID";
            col01[1] = "MOD_SEQ";
            col02[0] = AppData.ClassSatoo.AspxGetSESS_ID().ToString();
            col02[1] = "-1";

            var ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "WORK_TORIWRK0", 0, "0",
                                               col01.ToArray(), col02.ToArray());
            if (ret_val.Code < 0)
            {
                Mess2 = "NO取得エラー !";
                return;
            }
            var v_start = DateTime.Now;
            long new_seq = ret_val.NewSeq;
            /* 明細チェック用 */
            var err_wrk = new BizArray();
            /* 各明細の登録処理 */
            Text20 = 3;
            var ret_detail = AspxParaGetDetail(Inp72TranDetails, col_list2, new_seq, Text20,
                                               Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd"), err_wrk);
            /* テスト：2004/11/10 140明細=7sec */
            if (ret_detail < 0)
            {
                Mess2 = "明細登録エラー ! 赤い項目の[一覧]から選択して下さい｡";
                //OnErrCells
                return;
            }
            Text21 = new_seq;
            if (string.IsNullOrEmpty(Inp72DetailOpt.InpCd.Code))
                Inp72DetailOpt.InpCd = new BtListHelper(AppData.ClassSatoo.SHAIN_CD, AppData.ClassSatoo.SHAIN_Name);

            //AspxParaGet
            col02[0] = Inp72DetailOpt.ManualInpNo; // 手入力伝票NO
            col02[1] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd"); //在庫計上日
            col02[2] = Inp72DetailOpt.DeliverDate?.ToString("yyyyMMdd"); //納品日
            col02[3] = Inp72DetailOpt.TranCate; //取引区分
            col02[4] = Inp72DetailOpt.InpCd.Code; //入力社員CD
            col02[5] = Inp72DetailOpt.RecDestCd.Code; //取引先CD2
            col02[6] = Inp72DetailOpt.SupplierCd.Code; //取引先CD1
            col02[7] = Text10.ToString(); //掛率1
            col02[8] = Text11.ToString(); //外税対象金額
            col02[9] = Inp72DetailOpt.Num.ToString(); //数量合計
            col02[10] = Text13.ToString(); // 明細金額合計
            col02[11] = Text14.ToString(); // 内税消費税
            col02[12] = Text15.ToString(); // 外税消費税
            col02[13] = Inp72DetailOpt.RetailPrice.ToString(); // 上代合計
            col02[14] = Inp72DetailOpt.WholesalePrice.ToString(); // 下代合計
            col02[15] = Inp72DetailOpt.Memo; // メモ
            col02[16] = Text19.ToString(); // 掛計上FLG
            col02[17] = Text20.ToString(); // 伝票処理区分
            col02[18] = Text21.ToString(); // MOD_SEQ
            col02[19] = Text22.ToString(); // 倉庫CD
            col02[20] = Inp72DetailOpt.RelatedNo; // 関連伝票NO
            col02[21] = Inp72DetailOpt.RelatedNo2; // 関連伝票NO2
            /* 2022.02.09 倉庫CDは倉庫CDに設定 */
            col02[19] = Inp72DetailOpt.RecDestCd.Code;
            ret_val = AppData.Http!.AspxSqlExe(DBDef.DB_DML.INSERT, "TRAN_VTORI0", 0, "0",
                                               col_list.ToArray(), col02.ToArray());

            if (ret_val.Code == 0)
            {
                Inp72TranHead tranHead = new Inp72TranHead();
                tranHead.SeqNo = int.TryParse(ret_val.NewSeq.ToString(), out var _seqno) ? _seqno : 0;
                double vdate = double.TryParse(ret_val.VDate.ToString(), out var _vdate) ? _vdate : 0;
                tranHead.VdateCreate = vdate;
                tranHead.VdateUpdate = vdate;

                tranHead.ManualInpNo = col02[0];
                tranHead.InvCountDate = col02[1];
                tranHead.DeliverDate = col02[2];
                tranHead.TranCate = col02[3];
                tranHead.InpStaffCD = col02[4];
                tranHead.ClientCD2 = col02[5];
                tranHead.ClientCD1 = col02[6];
                tranHead.MarkupRate1 = int.TryParse(col02[7], out int mark_rate1) ? mark_rate1 : 0;
                tranHead.TaxableAmount = long.TryParse(col02[8], out long tax_amt) ? tax_amt : 0;
                tranHead.TotalNum = int.TryParse(col02[9], out int total_num) ? total_num : 0;
                tranHead.TotalDetailAmt = long.TryParse(col02[10], out long detail_amt) ? detail_amt : 0;
                tranHead.TaxIncluded = long.TryParse(col02[11], out long tax_include) ? tax_include : 0;
                tranHead.TaxExcluded = long.TryParse(col02[12], out long tax_exclude) ? tax_exclude : 0;
                tranHead.TotalRetail = long.TryParse(col02[13], out long retail) ? retail : 0;
                tranHead.TotalWholesale = long.TryParse(col02[14], out long wholesale) ? wholesale : 0;
                tranHead.Memo = col02[15];
                tranHead.AccFlg = int.TryParse(col02[16], out int acc_flg) ? acc_flg : 0;
                tranHead.SlipCate = int.TryParse(col02[17], out int slip_cate) ? slip_cate : 0;
                tranHead.ModSeq = long.TryParse(col02[18], out long moq_seq) ? moq_seq : 0;
                tranHead.StoreCD = col02[19];
                tranHead.RelatedSlipNo = long.TryParse(col02[20], out long slip_no) ? slip_no : 0;
                tranHead.RelatedSlipNo2 = long.TryParse(col02[21], out long slip_no2) ? slip_no2 : 0;

                tranHead.StaffName = Inp72DetailOpt.InpCd.Name;
                tranHead.SupplierName = Inp72DetailOpt.SupplierCd.Name;
                tranHead.CustomerName = Inp72DetailOpt.RecDestCd.Name;
                Inp72TranHeads.Add(tranHead);

                SelectedTabIndex = 0;
                System.Windows.MessageBox.Show("データ追加しました (" + AppData.ClassSatoo.GetDateDiff(v_start, DateTime.Now) + ")",
                                                   "確認", System.Windows.MessageBoxButton.OK);
            }
            else if (ret_val.Code == -1) Mess2 = "ロックエラーです";
            else if (ret_val.Code == -2) Mess2 = "他で更新されていますので、登録されていません";
            else Mess2 = "データ追加できませんでした";
        }
        /// <summary>
        /// 印刷(F9)
        /// </summary>
        [RelayCommand]
        private async Task DoDetailPrint()
        {
            if (Inp72DetailOpt == null || Inp72DetailOpt?.SlipNo == "" || Inp72DetailOpt?.SlipNo == "0") return;
            CSV_flg = 0;
            var wrk_para = new BizArray();
            wrk_para[0] = Inp72DetailOpt.SlipNo; /* 伝票NO */
            wrk_para[1] = Inp72DetailOpt.TranCate; /* 取引区分 */

            Mess2 = "スプール中です";
            var ret_csv = OnQueryDetailPrint(wrk_para, 0); /*返されたパラメータをret_csvに代入*/
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
        /// <summary>
        /// 一覧へ(ESC)
        /// </summary>
        [RelayCommand]
        private void DoExit()
        {
            SelectedTabIndex = 0;
        }
        #endregion

        #region DataGrid Events
        /// <summary>
        /// 商品CD TextChanged
        /// </summary> 
        [RelayCommand]
        private void DoProdChanged(Inp72TranDetail value)
        {
            if (AppData.ClassCvnet.MeickFlg == 0) return;
            var v_para = new BizArray();
            v_para[0] = value.ProdCD;
            var ret_para = AppData.ClassCvnet.GetShohin(v_para.ToArray());
            if (ret_para.Length == 0)
            {
                value.DetailName = string.Empty;
                Mess2 = "商品マスタに存在しません";
            }
            else
            {
                BizArray para = new BizArray(ret_para);
                OnZoomRet(value, para, 1);
                Mess2 = "";
            }
        }

        /// <summary>
        /// 商品CD 一覧Button
        /// </summary> 
        [RelayCommand]
        private void DoProdRowSearch(Inp72TranDetail value)
        {
            var ar2 = new BizArray();
            /* セールマスタ参照用 色、サイズ、在庫計上日、店舗 */
            if (string.IsNullOrEmpty(value.ColorCD)) ar2[0] = ".";
            else ar2[0] = value.ColorCD;
            if (string.IsNullOrEmpty(value.SizeCD)) ar2[1] = ".";
            else ar2[1] = value.SizeCD;
            ar2[2] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
            ar2[3] = Inp72DetailOpt.RecDestCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) ar2[2] = "19000101";

            if (AppData.ClassCvnet.MstDialog.ContainsKey("商品") && AppData.ClassCvnet.ComboListFlg3 == 1)
            {
                var ar = new BizArray();
                ar[0] = "0";

                var wrk_para = new BizArray();
                wrk_para[0] = value.ProdCD;
                wrk_para[3] = value.CostFlg.ToString();
                wrk_para[4] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
                if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para[4] = "19000101";

                List<CsvItem> def = new List<CsvItem>();
                CsvItem csv_item = new CsvItem();
                csv_item.col01 = "仕入先";
                csv_item.col02 = Inp72DetailOpt.SupplierCd.Code;
                csv_item.col03 = Inp72DetailOpt.SupplierCd.Code;
                def.Add(csv_item);

                var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, wrk_para.ToArray(), ar.ToArray(), def, ar2.ToArray());
                if (vm != null)
                {
                    var data = vm.SelShoResult0;
                    OnZoomRet(value, vm.SelShoResult0, 1);
                }
                return;
            }
            else
            {
                var wrk_para = new BizArray();
                wrk_para[0] = value.ProdCD;
                wrk_para[1] = "0";
                wrk_para[2] = Inp72DetailOpt.SupplierCd.Code;
                wrk_para[3] = value.ColorCD.ToString();
                wrk_para[4] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
                if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para[4] = "19000101";
                AppData.DlgService.GetSel2(wrk_para.ToArray(), null, null, ar2.ToArray());
            }

            value.ColorCD = string.Empty; /* 色 */
            value.SizeCD = string.Empty; /* サイズ */
            value.ColorName = string.Empty; /* 色名 */
            value.SizeName = string.Empty; /* サイズ名 */
        }

        /// <summary>
        /// 商品名 MouseDoubleClick
        /// </summary>
        [RelayCommand]
        private async Task DoPrintProdDClick(Inp72TranDetail value)
        {
            try
            {
                /* 品番印刷 */
                var wrk_para = new BizArray();
                wrk_para[0] = value.ProdCD;
                Mess2 = "スプール中です";

                string ret_csv = AppData.ClassCvnet.OnQueryPrintShohinCsv(wrk_para.ToArray(), 2);
                var lines = ret_csv.Split('\n');

                if (lines.Length < 2 || lines[1] == "0")
                    return;

                string pdfPath = lines[0];
                string url = AppData.Http.URLroot + pdfPath + "/data.pdf";

                bool ready = await Utils.GlobalFunc.WaitForPdfAsync(url, TimeSpan.FromSeconds(30));
                if (!ready)
                {
                    ClientLib.MessageBoxError(this, "Data生成に時間がかかりすぎています。\n 条件を絞ってください。");
                    return;
                }

                var win = new WebpdfView();
                if (win.DataContext is WebpdfViewModel vm)
                {
                    vm.Pdfdata = url;
                }
                Mess2 = "印刷しました";
                ClientLib.CursorToNormal();
                ClientLib.ShowDialogView(win, this);
            }
            catch (Exception ex) { }
        }

        /// <summary>
        /// 色CD 一覧Button
        /// </summary> 
        [RelayCommand]
        private void DoColorRowSearch(Inp72TranDetail value)
        {
            if (Inp72DetailOpt == null) return; 
            var wrk_para = new BizArray();
            wrk_para[0] = value.ProdCD;
            var wrk_para2 = new BizArray();
            wrk_para2[0] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
            wrk_para2[1] = Inp72DetailOpt.RecDestCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para2[0] = "19000101";
            var vm_result = AppData.DlgService.GetSelclsz0(wrk_para.ToArray(), wrk_para2.ToArray());
            if (vm_result != null) 
                OnZoomRet(value, vm_result.ret_para, 2); 
        }

        /// <summary>
        /// サイズ TextChanged
        /// </summary>
        [RelayCommand]
        private void DoProdSizChanged(Inp72TranDetail value)
        { 
            if (AppData.ClassCvnet.MeickFlg == 0) return;
            var v_para = new BizArray();
            v_para[0] = value.ProdCD;
            v_para[1] = value.ColorCD;
            v_para[2] = value.SizeCD;
            var ret_para = AppData.ClassCvnet.GetColSiz(v_para.ToArray());
            if (ret_para.Length == 0)
            {
                value.ColorName = "";
                value.SizeName = "";
                Mess2 = "商品色サイズマスタに存在しません";
            }
            else 
            {
                BizArray para = new BizArray(ret_para);
                OnZoomRet(value, para, 3);
                Mess2 = "";
            }
        }

        /// <summary>
        /// サイズ 一覧Button
        /// </summary>
        [RelayCommand]
        private void DoSizeRowSearch(Inp72TranDetail value)
        {
            var wrk_para = new BizArray();
            wrk_para[0] = value.ProdCD;
            wrk_para[1] = value.ColorCD;
            var wrk_para2 = new BizArray();
            wrk_para2[0] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd");
            wrk_para2[1] = Inp72DetailOpt.RecDestCd.Code;
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) wrk_para2[0] = "19000101";
            var vm_result = AppData.DlgService.GetSelclsz0(wrk_para.ToArray(), wrk_para2.ToArray());
            if (vm_result != null)
            {
                OnZoomRet(value, vm_result.ret_para, 3);
            }
        }

        /// <summary>
        /// 数量 TextChanged
        /// </summary>
        [RelayCommand]
        private void DoProdQntChanged(Inp72TranDetail value)
        {
            //Kirisute08
            OnChangeValue(value);
            OnChangeTotal(value);
        }

        /// <summary>
        /// 上代単価 TextChanged
        /// </summary>
        [RelayCommand]
        private void DoRetailChanged(Inp72TranDetail value)
        {
            //Kirisute13
            OnChangeValue(value);
        }

        /// <summary>
        /// 下代単価 TextChanged
        /// </summary>
        [RelayCommand]
        private void DoWholesaleChanged(Inp72TranDetail value)
        {
            //Kirisute15
            OnChangeValue(value);
        }
          
        /// <summary>
        /// </summary> 
        /// <param name="mode">1 商品CD, 2 色CD, 3 サイズCD</param>
        private void OnZoomRet(Inp72TranDetail selected, BizArray para, int mode)
        {
            if (selected == null || para == null) return;

            // 2 色CD, 3 サイズCD
            if (mode == 2 || mode == 3)
            {
                /* 色CD,サイズCD,色名,サイズ名,上代 */
                selected.ColorCD = para[0];
                selected.SizeCD = para[1];
                selected.ColorName = para[2];
                selected.SizeName = para[3];
                selected.RetailUnit = long.TryParse(para[4], out var _retail) ? _retail : 0;

                var wrk_para = new BizArray();
                wrk_para[0] = selected.ProdCD;
                wrk_para[1] = para[0];
                wrk_para[2] = para[1];
                string sql_query = "SELECT CASE WHEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD=:1 AND 色CD=:2 AND サイズCD=:3) IS NULL OR ";
                sql_query += "((SELECT 仕入価格 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD=:1 AND 色CD=:2 AND サイズCD=:3)=0) ";
                sql_query += "THEN 仕入価格 ELSE (SELECT 仕入価格 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD=:1 AND 色CD=:2 AND サイズCD=:3) END AS 仕入価格 from HC$MASTER_SHOHIN where  商品CD=:1";
                var ret_csv = AppData.Http?.AspxSqlQuery(sql_query, wrk_para.ToArray());
                if (ret_csv != null && ret_csv.Rows.Count > 0)
                    selected.WholesalesUnit = long.TryParse(ret_csv?.Rows[0][0].ToString(), out long _wholesales) ? _wholesales : 0;
            }
            // 1 商品CD
            else if (mode == 1)
            { 
                selected.ProdCD = para[0];
                selected.DetailName = para[1];
                selected.RetailUnit = long.TryParse(para[2], out long _retail) ? _retail : 0;
                /* 仕入値に変更 2008.06.17 */
                selected.WholesalesUnit = long.TryParse(para[15], out long _wholesales) ? _wholesales : 0;
                selected.MrkProdNum = para[17];
            }
            OnChangeValue(selected);
        }
        #endregion

        #region Functions
        private int OnCheckMst()
        {
            if (Inp72DetailOpt == null) return -1;
            /* 得意先チェック */
            if (Inp72DetailOpt.RecDestCd?.Code != "" && Inp72DetailOpt.RecDestCd?.pre_data?.Code != Inp72DetailOpt.RecDestCd?.Code)
            {
                string v_sqlstr = "select 得意先CD,得意先名 from HC$master_TOKUI where 得意先CD=:1  and 在庫管理FLG=1 and 店種区分<9";
                var v_array = new BizArray();
                v_array[0] = Inp72DetailOpt.RecDestCd.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv?.Rows.Count > 0)
                {
                    Inp72DetailOpt.RecDestCd = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp72DetailOpt.RecDestCd.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp72DetailOpt.RecDestCd = new BtListHelper("", "");
                    System.Windows.MessageBox.Show("得意先CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            /* 仕入先チェック */
            if (Inp72DetailOpt.SupplierCd?.Code != "" && Inp72DetailOpt.SupplierCd?.pre_data?.Code != Inp72DetailOpt.SupplierCd?.Code)
            {
                string v_sqlstr = "select 仕入先CD,仕入先名,掛率,掛率2,消費税CD,消費税計算方法,消費税端数 from HC$master_siire where 仕入先CD=:1";
                var v_array = new BizArray();
                v_array[0] = Inp72DetailOpt.SupplierCd.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv?.Rows.Count > 0)
                {
                    Text26 = int.TryParse(wrk_csv.Rows[0][2].ToString(), out int _text26) ? _text26 : 0;
                    Text27 = int.TryParse(wrk_csv.Rows[0][3].ToString(), out int _text27) ? _text27 : 0;
                    Text28 = int.TryParse(wrk_csv.Rows[0][4].ToString(), out int _text28) ? _text28 : 0;
                    Text29 = int.TryParse(wrk_csv.Rows[0][5].ToString(), out int _text29) ? _text29 : 0;
                    Text30 = int.TryParse(wrk_csv.Rows[0][6].ToString(), out int _text30) ? _text30 : 0;
                    if (Inp72DetailOpt.TranCate == "11" || Inp72DetailOpt.TranCate == "21") Text10 = Text27;
                    else Text10 = Text26;
                    Inp72DetailOpt.SupplierCd = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp72DetailOpt.SupplierCd.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp72DetailOpt.SupplierCd = new BtListHelper("", "");
                    System.Windows.MessageBox.Show("仕入先CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            /* 入力者チェック 21.10.13 */
            if (Inp72DetailOpt.InpCd?.Code != "" && Inp72DetailOpt.InpCd?.pre_data?.Code != Inp72DetailOpt.InpCd?.Code)
            {
                string v_sqlstr = "select 社員CD,名前 from HC$MASTER_SHAIN where 社員CD=:1";
                var v_array = new BizArray();
                v_array[0] = Inp72DetailOpt.InpCd.Code;
                var wrk_csv = AppData.Http?.AspxSqlQuery(v_sqlstr, v_array.ToArray());
                if (wrk_csv?.Rows.Count > 0)
                {
                    Inp72DetailOpt.InpCd = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                    Inp72DetailOpt.InpCd.pre_data = new BtListHelper(wrk_csv.Rows[0][0].ToString(), wrk_csv.Rows[0][1].ToString());
                }
                else
                {
                    Inp72DetailOpt.InpCd = new BtListHelper("", "");
                    System.Windows.MessageBox.Show("入力者CDがマスタに存在しません｡", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
            }
            return 0;
        }

        private void OnSetMultiRow(List<BizArray> set_para, string v_para = "")
        {
            /* データ列の並びを固定するようにする */
            foreach (var para in set_para)
            {
                Inp72TranDetail item = new Inp72TranDetail();
                item.ProdCD = para[0]; /* 商品CD */
                item.DetailName = para[1]; /* 商品名 */
                item.ColorCD = para[13]; /* 色CD */
                item.SizeCD = para[14]; /* サイズCD */
                item.ColorName = para[15]; /* 色名 */
                item.SizeName = para[16]; /* サイズ名 */
                item.Num = int.TryParse(para[18], out int _num) ? _num : 0; /* 数量 */

                item.RetailUnit = long.TryParse(para[23], out long _retail_unit) ? _retail_unit : 0; /* 上代 */
                /* 仕入値に変更 2008.06.17 */
                item.WholesalesUnit = long.TryParse(para[29], out long _wholesale_unit) ? _wholesale_unit : 0; /* 下代 */

                item.TaxCalcMethod = int.TryParse(para[9], out int _cal_method) ? _cal_method : 0; /* 消費税計算方法 */
                item.MrkProdNum = para[33]; /* メーカー品番 */

                item.CostFlg = int.TryParse(para[35], out int _cost_flg) ? _cost_flg : 0; /* 原価FLG */

                /* 明細合計計算 */
                item.UnitPrice = item.WholesalesUnit;
                item.RetailAmount = item.Num * item.RetailUnit;
                item.WholesalesAmount = item.Num * item.WholesalesUnit;
                item.TotalAmount = item.Num * item.UnitPrice;

                OnChangeTotal(item);
                Inp72TranDetails.Add(item);
            }
        }

        private void DoRefresh(Inp72TranDetail value)
        {
            /* 06.03.16 パラメータ変更 */
            var v_wkpara = new BizArray();
            v_wkpara[0] = value.ProdCD; /* 商品CD */
            v_wkpara[1] = value.ColorCD; /* 色 */
            v_wkpara[2] = value.SizeCD; /* サイズ */

            /* 上代表示変更対応 20080708 */
            v_wkpara[3] = Inp72DetailOpt.OrderDate?.ToString("yyyyMMdd"); /* 在庫計上日 */
            v_wkpara[4] = Inp72DetailOpt.RecDestCd.Code; /* 倉庫 */
            v_wkpara[5] = value.ProdCD; /* 商品CD */
            v_wkpara[6] = value.ColorCD; /* 色 */
            if (AppData.ClassCvnet.config.jodaihyjflg == 0) v_wkpara[3] = "19000101";

            var ret_csv = OnQueryTanka(v_wkpara);
            value.DetailName = string.Empty;
            value.RetailUnit = 0;
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
                    value.RetailUnit = long.TryParse(ret_csv.Rows[i][2].ToString(), out var retail_unit) ? retail_unit : 0;
                    value.WholesalesUnit = long.TryParse(ret_csv.Rows[i][3].ToString(), out var wholesale_unit) ? wholesale_unit : 0;
                    value.RetailAmount = value.Num * value.RetailUnit;
                    value.WholesalesAmount = value.Num * value.WholesalesUnit;
                    OnChangeTotal(value);
                }
                else if (tanka == 1)
                {
                    wrk_cnt[1] = 1;
                    value.ColorName = ret_csv.Rows[i][1].ToString();
                }
                else if (tanka == 2)
                {
                    wrk_cnt[2] = 1;
                    value.SizeName = ret_csv.Rows[i][1].ToString();
                }
            }
            string wrk_str = "";
            if (wrk_cnt[0] == 0) wrk_str += "商品マスタ "; 
            if (wrk_cnt[1] == 0) wrk_str += "カラーマスタ "; 
            if (wrk_cnt[2] == 0) wrk_str += "サイズマスタ ";

            if (wrk_str.Length > 0)
            {
                wrk_str += "がありませんでした";
                Mess2 = wrk_str;
            }
        }

        private DataTable OnQueryTanka(BizArray v_para)
        {
            string shocd = v_para[0];
            string irocd = v_para[1];
            string saizcd = v_para[2];

            var sql_query = "select 0,商品名,GET_JODAI(:1,:2,:3,:4,:5) 上代,CASE WHEN (SELECT 仕入価格 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD='" + shocd + "' AND 色CD='" + irocd + "' AND サイズCD='" + saizcd + "' ) IS NULL OR ((SELECT 仕入価格 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD='" + shocd + "' AND 色CD='" + irocd + "' AND サイズCD='" + saizcd + "' )=0) THEN 仕入価格 ELSE (SELECT 仕入価格 FROM HC$MASTER_SHOHIN_JAN WHERE 商品CD='" + shocd + "'  AND 色CD='" + irocd + "' AND サイズCD='" + saizcd + "' ) END AS 仕入価格,消費税計算方法 from HC$MASTER_SHOHIN where 商品CD=:6 ";
            sql_query += " union select 1,名称,0,0,0 from (select 名称CD,名称 from HC$Master_MEISHO where 名称区分='COL') where 名称CD=:7 ";
            sql_query += " union select 2,名称,0,0,0 from (select GET_SIZENAME('" + v_para[0] + "',:3) 名称 from dual)";
            return AppData.Http.AspxSqlQuery(sql_query, v_para.ToArray());
        }

        private void OnChangeValue(Inp72TranDetail row)
        {
            row.RetailAmount = row.Num * row.RetailUnit;
            row.WholesalesAmount = row.Num * row.WholesalesUnit;
            OnChangeTotal(row);
        }

        private void OnChangeTotal(Inp72TranDetail row)
        {
            if (Inp72DetailOpt == null) return;
            if (Inp72TranDetails.Count == 0)
            {
                Inp72DetailOpt.Num = 0;
                Inp72DetailOpt.RetailPrice = 0;
                Inp72DetailOpt.WholesalePrice = 0;
                Text13 = 0;
                Text11 = 0;
                return;
            }
            int? total_su = 0;
            long? total_kin1 = 0;
            long? total_kin2 = 0;
            total_su = Inp72TranDetails.Sum(x => x.Num);
            total_kin1 = Inp72TranDetails.Sum(x => x.RetailAmount);
            total_kin2 = Inp72TranDetails.Sum(x => x.WholesalesAmount);
            foreach (var item in Inp72TranDetails)
            {
                item.UnitPrice = item.WholesalesUnit;
                item.TotalAmount = item.WholesalesAmount;
            }
            Inp72DetailOpt.Num = total_su;
            Inp72DetailOpt.RetailPrice = total_kin1;
            Inp72DetailOpt.WholesalePrice = total_kin2;
            Text13 = total_kin2 ?? 0;
            Text11 = total_kin2 ?? 0;
        }

        private void OnCellStdColor()
        {
            foreach (var row in Inp72TranDetails)
            {
                row.ProdBgColor = Brushes.White.Color.ToString();
                row.ColorBgColor = Brushes.White;
                row.SizeBgColor = Brushes.White;
            }
        }

        private int OnCheckError()
        {
            if (Inp72TranDetails.Count <= 0)
            { 
                System.Windows.MessageBox.Show("明細レコードがありません！", "確認", System.Windows.MessageBoxButton.OK);
                return -1;
            }
            if (Inp72DetailOpt?.SupplierCd?.Code == "")
            {
                System.Windows.MessageBox.Show("仕入先を入力して下さい！", "確認", System.Windows.MessageBoxButton.OK);
                return -1;
            }
            int idx = 0;
            foreach (var row in Inp72TranDetails)
            {
                if (row.ProdCD.Trim() == "")
                {
                    Inp72DetailSelIdx = idx;
                    System.Windows.MessageBox.Show("商品CDを入力して下さい！", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
                if (row.ColorCD.Trim() == "" && AppData.ClassCvnet.config.SKUFlg != 1)
                {
                    Inp72DetailSelIdx = idx;
                    System.Windows.MessageBox.Show("色CDを入力して下さい！", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
                if (row.SizeCD.Trim() == "" && AppData.ClassCvnet.config.SKUFlg != 1)
                {
                    Inp72DetailSelIdx = idx;
                    System.Windows.MessageBox.Show("サイズCDを入力して下さい！", "確認", System.Windows.MessageBoxButton.OK);
                    return -1;
                }
                row.TranCate = int.TryParse(Inp72DetailOpt.TranCate, out int _trancate) ? _trancate : 0;
                row.RelatedSlipNo = long.TryParse(Inp72DetailOpt.RelatedNo, out long _slip_no) ? _slip_no : 0;
                row.RelatedSlipLineNo = long.TryParse(Inp72DetailOpt.RelatedNo2, out long _slip_no2) ? _slip_no2 : 0;
                idx++;
            }
            /* 同一品番チェック */
            bool isUnique = false;
            if (Inp72TranDetails.Count > 1) 
                isUnique = Inp72TranDetails.GroupBy(p => p.ProdCD)
                                           .All(g => g.Count() == 1); 
            if (isUnique)
            {
                System.Windows.MessageBox.Show("異なる型番の商品があります。登録できません！", "確認", System.Windows.MessageBoxButton.OK);
                return -1;
            } 
            /* 件数チェック */
            if (Inp72TranDetails.Count > 100)
            {
                System.Windows.MessageBox.Show("行数が101以上存在します。101行目以降は別伝票で登録してください！", "確認", System.Windows.MessageBoxButton.OK);
                return -1;
            }
            Text11 = Text13; 
            return 0;
        }

        private int AspxParaGetDetail(ObservableCollection<Inp72TranDetail> v_obj,
                                      BizArray v_collist, long v_headno, int v_kubun, string v_date, BizArray err_wrk)
        {
            string header = "ヘッダNO,伝票処理区分,在庫計上日,行NO,";
            for (int i = 0; i < v_collist.Count; i++)
            {
                header += v_collist[i];
                if (i < (v_collist.Count - 1)) header += ",";
            }
            header += "\n";

            int line_no = 1;
            string csv_data = string.Empty;
            for (int i = 0; i < v_obj.Count; i++)
            {
                csv_data += v_headno + ","; // ヘッダNO
                csv_data += v_kubun + ","; // 伝票処理区分
                csv_data += v_date + ","; // 在庫計上日
                csv_data += line_no + ","; // 行NO
                csv_data += v_obj[i].TranCate + ","; // 明細取引区分
                csv_data += v_obj[i].ProdCD + ","; // 商品CD
                csv_data += v_obj[i].ColorCD + ","; // 色CD
                csv_data += v_obj[i].SizeCD + ","; // サイズCD
                csv_data += v_obj[i].DetailName + ","; // 明細名称
                csv_data += v_obj[i].Num + ","; // 数量
                csv_data += v_obj[i].UnitPrice + ","; // 単価
                csv_data += v_obj[i].TotalAmount + ","; // 金額
                csv_data += v_obj[i].TaxIncluded + ","; // 内税消費税
                csv_data += v_obj[i].TaxExcluded + ","; // 外税消費税
                csv_data += v_obj[i].RetailUnit + ","; // 上代単価
                csv_data += v_obj[i].RetailAmount + ","; // 上代金額
                csv_data += v_obj[i].WholesalesUnit + ","; // 下代単価
                csv_data += v_obj[i].WholesalesAmount + ","; //下代金額
                csv_data += v_obj[i].DetailMemo + ","; // 明細メモ
                csv_data += v_obj[i].TaxCalcMethod + ","; // 消費税計算方法
                csv_data += v_obj[i].ProdSerial + ","; // 商品シリアル
                csv_data += v_obj[i].RelatedSlipNo + ","; // 関連伝票NO
                csv_data += v_obj[i].RelatedSlipLineNo + ","; // 関連伝票行NO
                csv_data += v_obj[i].Jancode + ","; // JANCODE
                csv_data += v_obj[i].CostFlg + ","; // 原価FLG
                csv_data += v_obj[i].CompleteFlg; // 完了FLG 
                if (i < (v_obj.Count - 1)) csv_data += "\n";
                line_no++;
            }

            var v_para = new BizArray();
            v_para[0] = "WORK_TORIWRK1";
            v_para[1] = header + csv_data;
            var ret_csv2 = AppData.Http!.AspxSqlQuery2("mi", v_para.ToArray());
            /* 06.03.29 エラーチェック追加 */
            if (!string.IsNullOrEmpty(ret_csv2))
            {
                string[] line_csv2 = ret_csv2.Split('\n');
                if (line_csv2[0] != "0")
                {
                    if (err_wrk != null)
                    {
                        for (int i = 2; i < line_csv2.Length; i++)
                        {
                            err_wrk[i - 2] = line_csv2[i];
                        }
                    }
                    return -1;
                }
            }
            return 0;
        }
        #endregion
    }

    public partial class Inp72SearchOpt : ObservableObject
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
        /// 納品日 Title
        /// </summary>
        [ObservableProperty]
        string? m_LblDeliverDate;

        /// <summary>
        /// 納品日 - START
        /// </summary>
        [ObservableProperty]
        DateTime? m_DeliverStartDate;

        /// <summary>
        /// 納品日 - END
        /// </summary>
        [ObservableProperty]
        DateTime? m_DeliverEndDate;

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
        /// 仕入先 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SupplierStartCd;

        /// <summary>
        /// 仕入先 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SupplierEndCd;

        /// <summary>
        /// 入庫先 - START
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_RecDestStartCd;

        /// <summary>
        /// 入庫先 - END
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_RecDestEndCd;

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
        /// 一覧 / 明細 Radio Option [0 一覧, 1 明細]
        /// </summary>
        [ObservableProperty]
        int? m_SelPrintCond;

        /// <summary>
        /// CSV出力 Title
        /// </summary>
        [ObservableProperty]
        string? m_CsvExportTitle;

        public Inp72SearchOpt()
        {
            SlipStartNo = "0";
            SlipEndNo = "99999999";
            DeliverStartDate = new DateTime(1901, 1, 1);
            DeliverEndDate = new DateTime(2099, 12, 31);
            TranStartCat = "00";
            TranEndCat = "99";
            RelatedStartNo = "0";
            RelatedEndNo = "9999999999999";
            RelatedStartNo2 = "0";
            RelatedEndNo2 = "9999999999999";
            ManualInpStartNo = "";
            ManualInpEndNo = "゜";
            SupplierStartCd = new BtListHelper("", "");
            SupplierEndCd = new BtListHelper("99999999", "");
            RecDestStartCd = new BtListHelper("", "");
            RecDestEndCd = new BtListHelper("99999999", "");
            ProdStartCd = new BtListHelper("", "");
            ProdEndCd = new BtListHelper("ZZZZZZZZZZZZZZZZ", "");
            InpStartCd = new BtListHelper("", "");
            InpEndCd = new BtListHelper("99999999", "");
            CsvExportTitle = "CSV出力";
        }
    }

    public partial class Inp72TranHead : ObservableObject
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
        /// 手入力伝票NO
        /// </summary>
        [ObservableProperty]
        string? m_ManualInpNo;

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
        /// 取引先CD2
        /// </summary>
        [ObservableProperty]
        string? m_ClientCD2;

        /// <summary>
        /// 取引先CD1
        /// </summary>
        [ObservableProperty]
        string? m_ClientCD1;

        /// <summary>
        /// 掛率1
        /// </summary>
        [ObservableProperty]
        int? m_MarkupRate1;

        /// <summary>
        /// 外税対象金額
        /// </summary>
        [ObservableProperty]
        long? m_TaxableAmount;

        /// <summary>
        /// 数量合計
        /// </summary>
        [ObservableProperty]
        int? m_TotalNum;

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
        /// 掛計上FLG
        /// </summary>
        [ObservableProperty]
        int? m_AccFlg;

        /// <summary>
        /// 伝票処理区分
        /// </summary>
        [ObservableProperty]
        int? m_SlipCate;

        /// <summary>
        /// MOD_SEQ
        /// </summary>
        [ObservableProperty]
        long? m_ModSeq;

        /// <summary>
        /// 倉庫CD
        /// </summary>
        [ObservableProperty]
        string? m_StoreCD;

        /// <summary>
        /// 関連伝票NO
        /// </summary>
        [ObservableProperty]
        long? m_RelatedSlipNo;

        /// <summary>
        /// 関連伝票NO2
        /// </summary>
        [ObservableProperty]
        long? m_RelatedSlipNo2;

        /// <summary>
        /// 担当名
        /// </summary>
        [ObservableProperty]
        string? m_StaffName;

        /// <summary>
        /// 仕入先名
        /// </summary>
        [ObservableProperty]
        string? m_SupplierName;

        /// <summary>
        /// 得意先名
        /// </summary>
        [ObservableProperty]
        string? m_CustomerName;

        /// <summary>
        /// 掛率
        /// </summary>
        [ObservableProperty]
        int? m_MarkupRate;

        /// <summary>
        /// 掛率2
        /// </summary>
        [ObservableProperty]
        int? m_MarkupRate2;

        /// <summary>
        /// 消費税CD
        /// </summary>
        [ObservableProperty]
        long? m_TaxCD;

        /// <summary>
        /// 消費税計算方法
        /// </summary>
        [ObservableProperty]
        int? m_TaxCalcMethod;

        /// <summary>
        /// 消費税端数
        /// </summary>
        [ObservableProperty]
        int? m_TaxRound;
    }

    public partial class Inp72DetailOpt : ObservableObject
    {
        /// <summary>
        /// 伝票No
        /// </summary>
        [ObservableProperty]
        string? m_SlipNo;

        /// <summary>
        /// 発注日
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
        /// 仕入先
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_SupplierCd;

        /// <summary>
        /// 手入力No
        /// </summary>
        [ObservableProperty]
        string? m_ManualInpNo;

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
        /// 入庫先
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_RecDestCd;

        /// <summary>
        /// 入力者
        /// </summary>
        [ObservableProperty]
        BtListHelper? m_InpCd;

        /// <summary>
        /// 備考
        /// </summary>
        [ObservableProperty]
        string? m_Memo;

        /// <summary>
        /// 展開商品CD
        /// </summary>
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
        long? m_RetailPrice;

        /// <summary>
        /// 計 (下代金額)
        /// </summary>
        [ObservableProperty]
        long? m_WholesalePrice;

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

        public Inp72DetailOpt()
        {
            SlipNo = string.Empty;
            OrderDate = DateTime.Now;
            DeliverDate = DateTime.Now;
            TranCate = string.Empty;
            SupplierCd = new BtListHelper("", "");
            ManualInpNo = string.Empty;
            RelatedNo = string.Empty;
            RelatedNo2 = string.Empty;
            if (AppData.ClassCvnet.SysImp != null)
                RecDestCd = new BtListHelper(AppData.ClassCvnet.SysImp.Rows[0][1].ToString(),
                                             AppData.ClassCvnet.SysImp.Rows[0][2].ToString());
            else RecDestCd = new BtListHelper("", "");
            RecDestCd.pre_data = new BtListHelper("", "");
            InpCd = new BtListHelper(AppData.ClassSatoo.SHAIN_CD, AppData.ClassSatoo.SHAIN_Name);
            Memo = string.Empty;
            ProdCd = new BtListHelper("", "");
            Num = 0;
            RetailPrice = 0;
            WholesalePrice = 0;
            CreatedAt = string.Empty;
            UpdatedAt = string.Empty;
        }
    }

    public partial class Inp72TranDetail : ObservableObject
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
        /// 明細取引区分
        /// </summary>
        [ObservableProperty]
        int? m_TranCate;

        /// <summary>
        /// 商品CD
        /// </summary>
        [ObservableProperty]
        string? m_ProdCD;

        /// <summary>
        /// 商品CD Background Color
        /// </summary>
        [ObservableProperty]
        string? m_ProdBgColor;

        /// <summary>
        /// 色CD
        /// </summary>
        [ObservableProperty]
        string? m_ColorCD;

        /// <summary>
        /// 色CD Background Color
        /// </summary>
        [ObservableProperty]
        Brush? m_ColorBgColor;

        /// <summary>
        /// サイズCD
        /// </summary>
        [ObservableProperty]
        string? m_SizeCD;

        /// <summary>
        /// サイズCD Background Color
        /// </summary>
        [ObservableProperty]
        Brush? m_SizeBgColor;

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
        long? m_UnitPrice;

        /// <summary>
        /// 金額
        /// </summary>
        [ObservableProperty]
        long? m_TotalAmount;

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
        long? m_RetailUnit;

        /// <summary>
        /// 上代金額
        /// </summary>
        [ObservableProperty]
        long? m_RetailAmount;

        /// <summary>
        /// 下代単価
        /// </summary>
        [ObservableProperty]
        long? m_WholesalesUnit;

        /// <summary>
        /// 下代金額
        /// </summary>
        [ObservableProperty]
        long? m_WholesalesAmount;

        /// <summary>
        /// 明細メモ
        /// </summary>
        [ObservableProperty]
        string? m_DetailMemo;

        /// <summary>
        /// 消費税計算方法
        /// </summary>
        [ObservableProperty]
        int? m_TaxCalcMethod;

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
        /// MKR品番
        /// </summary>
        [ObservableProperty]
        string? m_MrkProdNum;

        public Inp72TranDetail()
        {
            ProdBgColor = string.Empty;
            ColorBgColor = Brushes.White;
            SizeBgColor = Brushes.White;
        }
    }
}
