using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CvnetBaseCore;
using CvnetClient.Models;
using CvnetClient.Utils;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;

namespace CvnetClient.ViewModels
{
    public partial class SubDlg01Sho3ViewModel : BaseViewModel
    {
        #region Binding Variable
        [ObservableProperty]
        Shohin3Opt? shohin3Opt;

        [ObservableProperty]
        ObservableCollection<MasterShohin>? listProduct;

        /// <summary>
		/// 修正用の一時的なProductオブジェクト
		/// </summary>
		[ObservableProperty]
        MasterShohin? editProduct;

        [ObservableProperty]
        MasterShohin? selectedProduct;

        [ObservableProperty]
        string? selProductCd;
        #endregion

        #region Class Variable 
        private long V_SEQ;
        private double V_UPDATE;
        private JanStyleRec JanStyleRec;
        private string nendo_format;

        /*照会モードFLG*/
        private int shokai_flg = 0;

        private BizArray sv_para;
        private string sv_sql;

        private BizArray col_list;

        private BizCsvDocument Ryakusho;
        #endregion

        [RelayCommand]
        void Init(object? init_para)
        {
            OnInitBase(init_para);
            Shohin3Opt = new Shohin3Opt();
            EditProduct = new MasterShohin();

            var comboItem = AppData.ClassCvnet.comboItem00;
            #region Set ComboList
            // Set 在庫管理 ComboList
            InvMngmentList = comboItem.ComboItem_00<int>("する");
            EditProduct.InvMngmentFLG = InvMngmentList.FirstOrDefault().Key;
            // Set セール区分 ComboList
            SalesCateList = new Dictionary<int, string>
            {
                {  0, "0 プロパー商品" },
                {  1, "1 セール商品" }
            };
            EditProduct.SalesCate = SalesCateList.FirstOrDefault().Key;
            // Set 売上基準自動補充 ComboList
            AutoDistList = new Dictionary<int, string>
            {
                {  0, "0 自動補充しない" },
                {  1, "1 自動補充する" }
            };
            EditProduct.AutoDistFLG = AutoDistList.FirstOrDefault().Key;
            // Set 納品区分 ComboList
            DeliverCateList = new Dictionary<int, string>
            {
                {  0, "0 倉庫入庫" },
                {  1, "1 店舗入庫" }
            };
            EditProduct.DeliveryCate = DeliverCateList.FirstOrDefault().Key;
            // Set EC連携 ComboList 
            EcConnectList = new Dictionary<int, string>
            {
                {  0, "0 連携しない" },
                {  1, "1 連携する" }
            };
            EditProduct.EcConnect = EcConnectList.FirstOrDefault().Key;
            // Set EC取置 ComboList
            EcReserveList = new Dictionary<int, string>
            {
                {  0, "0 取置しない" },
                {  1, "1 取置する" }
            };
            EditProduct.EcReserve = EcReserveList.FirstOrDefault().Key;
            // Set 仕入区分 ComboList
            PurchCateList = comboItem.ComboItem_00<int>("仕入区分");
            EditProduct.PurchaseCate = PurchCateList.FirstOrDefault().Key;
            // Set 消化計算 ComboList
            DgCalcList = new Dictionary<int, string>
            {
                {  0, "0 仕入価格代入" },
                {  1, "1 掛率計算" }
            };
            EditProduct.DgCalcCate = DgCalcList.FirstOrDefault().Key;
            // Set 消化桁切 ComboList
            DgCutOffList = comboItem.ComboItem_00<int>("桁切");
            EditProduct.DgCutOffSpec = DgCutOffList.FirstOrDefault().Key;
            // Set 消化端数 ComboList
            ConPurcCateList = comboItem.ComboItem_00<int>("端数");
            EditProduct.ConsignPurcCate = ConPurcCateList.FirstOrDefault().Key;
            // Set POS区分 ComboList
            PosCateList = new Dictionary<int, string>
            {
                {  0, "0 通常" },
                {  9, "9 POSﾏｽﾀ削除指示" },
                { 10, "10 出力しない" },
            };
            EditProduct.PosCate = PosCateList.FirstOrDefault().Key;
            // Set 代表品番FLG ComboList
            RepresNoFlgList = new Dictionary<int, string>
            {
                {  0, "0 通常商品" },
                {  1, "1 代表品番商品" }
            };
            EditProduct.RepresentNoFLG = RepresNoFlgList.FirstOrDefault().Key;
            // Set 商品区分FLG ComboList
            ProdCateFlgList = new Dictionary<int, string>
            {
                {  0, "0 通常" },
                {  1, "1 商品外" }
            };
            EditProduct.ProdCateFLG = ProdCateFlgList.FirstOrDefault().Key;
            // Set 消費税CD (軽減税率) ComboList
            TaxCDList = new Dictionary<int, string>
            {
                {  1, "1 通常税率" },
                {  2, "2 軽減税率" }
            };
            EditProduct.TaxCD = TaxCDList.FirstOrDefault().Key;
            // Set 商品サイズ区分 ComboList  
            var prod_list = new Dictionary<string, string>();
            string sql_query = "select 名称CD, 名称 from hc$master_meisho where 名称区分='IDX' and (名称CD like 'US%' or 名称CD='SIZ' ) order by 名称CD";
            var get_prodSiz = AppData.Http?.AspxSqlQuery(sql_query, null);
            foreach (DataRow row in get_prodSiz.Rows)
            {
                string? key = row[0] != DBNull.Value ? row[0].ToString() : string.Empty;
                string? value = row[1] != DBNull.Value ? row[1].ToString() : string.Empty;
                prod_list.Add(key, string.Format("{0} {1}", key, value));
            }
            ProdSizCateList = prod_list;
            EditProduct.ProdSizeCate = ProdSizCateList.FirstOrDefault().Key;
            // Set ゼロ単価区分 ComboList
            ZeroPriCateList = new Dictionary<int, string>
            {
                {  0, "0 禁止" },
                {  1, "1 許可" },
                {  2, "2 必須入力" }
            };
            EditProduct.ZeroPriceCate = ZeroPriCateList.FirstOrDefault().Key;
            // Set 消費税計算方法 ComboList
            TaxCalcList = comboItem.ComboItem_00<int>("課税区分");
            EditProduct.TaxCalcMethod = TaxCalcList.FirstOrDefault().Key;
            // Set コラボ出力区分 (下札サイズ) ComboList
            ColOutCateList = new Dictionary<int, string>
            {
                {  0, "0 禁止" },
                {  1, "1 許可" },
                {  2, "2 必須入力" }
            };
            EditProduct.CollabOutCate = ColOutCateList.FirstOrDefault().Key;
            #endregion

            //Get Column Name List 
            sql_collist = sql_collist.Replace("\r", "").Replace("\n", "").Trim();
            string[] wrk_list = sql_collist.Trim().Split(',');
            for (int i = 0; i < wrk_list.Length; i++)
            {
                wrk_list[i] = wrk_list[i].Trim();
            }
            col_list = (wrk_list.Length > 0) ? new BizArray(wrk_list) : new BizArray();

            var p_len = para.Count;

            JanStyleRec = new JanStyleRec();
            JanStyleRec.equalFlg = AppData.ClassCvnet.config.equalFlg;
            JanStyleRec.janlength1 = AppData.ClassCvnet.config.janlength1;
            JanStyleRec.janlength2 = AppData.ClassCvnet.config.janlength2;
            JanStyleRec.pttrnStyle = AppData.ClassCvnet.config.pttrnStyle;
            JanStyleRec.makejan = AppData.ClassCvnet.config.makejan;
            JanStyleRec.renbanflg = AppData.ClassCvnet.config.renbanflg;
            /* 20070215追加 */
            JanStyleRec.danflg = AppData.ClassCvnet.config.danflg;

            int addCol = 0;
            /* janpatternのセット */
            JanStyleRec.janPattern = new DataTable();
            if (!string.IsNullOrEmpty(AppData.ClassCvnet.config.janpattern))
            {
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
            }

            /* Regaxchkのセット */
            JanStyleRec.Regaxchk = new DataTable();
            if (!string.IsNullOrEmpty(AppData.ClassCvnet.config.Regaxchk))
            {
                addCol = 5;
                for (int i = 0; i < addCol; i++)
                {
                    JanStyleRec.Regaxchk.Columns.Add("COL" + i.ToString("00"), typeof(string));
                }
                var regrows = AppData.ClassCvnet.config.Regaxchk.Split("\n");
                for (int i = 0; i < regrows.Length; i++)
                {
                    var regcols = regrows[i].Split("@");
                    DataRow row = JanStyleRec.Regaxchk.NewRow();
                    for (int j = 0; j < regcols.Length; j++)
                    {
                        row["COL" + j.ToString("00")] = regcols[j];
                    }
                    JanStyleRec.Regaxchk.Rows.Add(row);
                }
            }

            /* barpatternのセット */
            if (JanStyleRec.danflg == 1)
            {
                JanStyleRec.barPattern = new DataTable();
                if (!string.IsNullOrEmpty(AppData.ClassCvnet.config.barpattern))
                {
                    addCol = 8;
                    for (int i = 0; i < addCol; i++)
                    {
                        JanStyleRec.barPattern.Columns.Add("COL" + i.ToString("00"), typeof(string));
                    }
                    var barrows = AppData.ClassCvnet.config.barpattern.Split("\n");
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
                }
            }

            /* janlength1-janlength2の値だけで判断する */
            if (JanStyleRec.equalFlg == 1)
            {
                Shohin3Opt.ProdMaxLen = JanStyleRec.janlength1 - 1;
            }
            if (JanStyleRec.janlength1 - JanStyleRec.janlength2 > 0)
            {
                Shohin3Opt.JanCode3Vis = Visibility.Visible;
                Shohin3Opt.Jan1stMaxLen = JanStyleRec.janlength2;
            }
            else if (JanStyleRec.janlength1 - JanStyleRec.janlength2 == 0)
            {
                Shohin3Opt.Jan1stMaxLen = JanStyleRec.janlength1;
                Shohin3Opt.JanCode3Vis = Visibility.Collapsed;
            }

            if (p_len > 0)
            {
                if (para[0] == "1")
                {
                    /* 追加専用モード */
                    Shohin3Opt.BtReDispAct = false;
                    Shohin3Opt.BtReDispNextAct = false;
                    Shohin3Opt.BtReDispPrevAct = false;
                    Shohin3Opt.BtUpdateAct = false;
                    Shohin3Opt.BtUpdateVis = Visibility.Collapsed;
                    /* 追加ボタン・商品CD修正OK */
                    Shohin3Opt.ProdReadOnly = false;
                    Shohin3Opt.BtInsertAct = true;
                    Shohin3Opt.BtInsertVis = Visibility.Visible;
                    /* JANキー項目入力不可 */
                    if (JanStyleRec.makejan == 0 && JanStyleRec.equalFlg == 0)
                        Shohin3Opt.Jan1stReadOnly = false;
                    else if (JanStyleRec.makejan == 1 && JanStyleRec.equalFlg == 1)
                        Shohin3Opt.ProdReadOnly = true;

                    /* 最新の展示会CDをセット 2008.09.25 追加 */
                    GetTenjikai();

                    /* JANCodeパターンによる画面設定*/
                    SetMode(para.ToArray());
                }
                else if (para[0] == "2")
                {
                    /* 追加、修正共用 */
                    Shohin3Opt.ProdReadOnly = false;
                    Shohin3Opt.BtInsertAct = true;
                    Shohin3Opt.BtInsertVis = Visibility.Visible;
                    /* JANキー項目入力不可 */
                    if (JanStyleRec.makejan == 0 && JanStyleRec.equalFlg == 0)
                        Shohin3Opt.Jan1stReadOnly = false;
                    else if (JanStyleRec.makejan == 1 && JanStyleRec.equalFlg == 1)
                        Shohin3Opt.ProdReadOnly = true;
                }
                else if (para[0] == "3")
                {
                    Shohin3Opt.BtInsertAct = false;
                    Shohin3Opt.BtInsertVis = Visibility.Collapsed;
                    Shohin3Opt.BtDeleteAct = false;
                    Shohin3Opt.BtDeleteVis = Visibility.Collapsed;
                    Shohin3Opt.BtUpdateAct = false;
                    Shohin3Opt.BtUpdateVis = Visibility.Collapsed;

                    /*全項目日非活性*/
                    shokai_flg = 1;
                    Shohin3Opt.ItemListAct = false;
                }
            }
            else SetMode();

            EditProduct.TaxCalcMethod = 1;
            EditProduct.InvMngmentFLG = 1;
            EditProduct.TaxCD = 1;
            EditProduct.PurchaseCate = 0;
            EditProduct.DgCutOffSpec = 0;
            EditProduct.ConsignPurcCate = 0;
            EditProduct.DgCalcCate = 0;

            var wrk_csv = AppData.Http?.AspxSqlQuery("select 名称CD,名称 from HC$master_meisho where 名称区分='IDX' and 名称CD between 'B01' and 'B10' order by 名称CD");
            if (wrk_csv != null)
            {
                for (int i = 0; i < wrk_csv.Rows.Count; i++)
                {
                    string name_cd = wrk_csv.Rows[i][0].ToString();
                    string idx = name_cd.Length > 1 ? name_cd.Substring(1) : "";
                    var prop = typeof(Shohin3Opt).GetProperty("LabelA" + idx);
                    if (prop != null)
                    {
                        prop.SetValue(Shohin3Opt, wrk_csv.Rows[i][1]);
                    }
                }
            }
            /* 各名称→JAN生成用のCSV、Ryakushoに保存 */
            string kbn = "";
            for (var i = 0; i < JanStyleRec.janPattern.Rows.Count; i++)
            {
                if (!string.IsNullOrEmpty(JanStyleRec.janPattern.Rows[i][5].ToString()))
                    kbn += (kbn == "") ? JanStyleRec.janPattern.Rows[i][5].ToString() : "','" + JanStyleRec.janPattern.Rows[i][5].ToString();
            }
            var _ryakusho = AppData.Http?.AspxSqlQuery("select 名称区分,名称CD,略称,名称 from HC$master_meisho where 名称区分 in ('" + kbn + "') order by 名称区分,名称CD");
            if (_ryakusho != null) Ryakusho = new BizCsvDocument(_ryakusho);

            /* ラベルをセット */
            wrk_csv = AppData.Http?.AspxSqlQuery("select 名称CD,名称 from HC$master_meisho where 名称区分='IDX' and 名称CD between 'Y01' and 'Y20' order by 名称CD");
            if (wrk_csv != null)
            {
                for (int i = 0; i < wrk_csv.Rows.Count; i++)
                {
                    string name_cd = wrk_csv.Rows[i][0].ToString();
                    string idx = name_cd.Length > 1 ? name_cd.Substring(1) : "";
                    var prop = typeof(Shohin3Opt).GetProperty("LabelB" + idx);
                    if (prop != null)
                    {
                        prop.SetValue(Shohin3Opt, wrk_csv.Rows[i][1]);
                    }
                }
            }

            /* 年度を指定 */
            if (AppData.ClassCvnet.config.NendoKeta != 0)
            {
                nendo_format = "";
                for (var i = 0; i < AppData.ClassCvnet.config.NendoKeta; i++)
                {
                    nendo_format += "0";
                }
                EditProduct.JanCode1 = DateTime.Now.Year.ToString().Substring(DateTime.Now.Year.ToString().Length - AppData.ClassCvnet.config.NendoKeta);
            }

            /*連携対応　09.01.06 */
            if (AppData.ClassCvnet.config.PosFlg != 0)
            {
                Shohin3Opt.BtRenkeiVis = Visibility.Visible;
                Shohin3Opt.BtRenkeiAct = true;
            }

            /*原価FLG対応　09.04.20 */
            if (AppData.ClassCvnet.config.usegenka == 1)
            {
                Shohin3Opt.LblPurchPriceVis = Visibility.Collapsed;
                Shohin3Opt.TxtPurchPriceVis = Visibility.Collapsed;
            }

            /* 最終仕入 09.12.08 */
            if (AppData.ClassCvnet.config.LastSiire != 0 || AppData.ClassCvnet.config.souheikin != 0)
            {
                Shohin3Opt.BtGenkaTitle = "原価履歴(S+F11)";
            }

            /* 照会用 */
            if (para[0] == "3")
            {
                Shohin3Opt.BtInsertAct = false;
                Shohin3Opt.BtInsertVis = Visibility.Collapsed;
                Shohin3Opt.BtDeleteAct = false;
                Shohin3Opt.BtDeleteVis = Visibility.Collapsed;
                Shohin3Opt.BtUpdateAct = false;
                Shohin3Opt.BtUpdateVis = Visibility.Collapsed;
            }

            /* 2010.12.16 外貨単価対応 S */
            Visibility visibled = Visibility.Collapsed;
            if (AppData.ClassCvnet.config.dispGaitan == 1) visibled = Visibility.Visible;
            Shohin3Opt.LblForCurPrVis = visibled;
            Shohin3Opt.TxtForCurPrVis = visibled;

            /* 2010.12.16 外貨単価対応 E */
            AppData.ClassCvnet.AspxSqlQueryImp();

            /* 16.02.08 #24937 CD欄消す */
            if (AppData.ClassCvnet.ComboListFLg == 1)
            {
                Shohin3Opt.DspMeicodeVis = Visibility.Collapsed;
                Shohin3Opt.ImpMeiCodeVis = Visibility.Collapsed;
            }
              
            /* 2021.07.15 ECフラグ S */
            visibled = Visibility.Collapsed;
            if (AppData.ClassCvnet.config.ECFLG == 1) visibled = Visibility.Visible;
            Shohin3Opt.LblEcConnectVis = visibled;
            Shohin3Opt.TxtEcConnectVis = visibled;
            Shohin3Opt.LblEcReserveVis = visibled;
            Shohin3Opt.TxtEcReserveVis = visibled; 
            /* 2021.07.15 ECフラグ E */
        }

        string sql_collist = """
               商品CD,商品名,略称,旧コード,展示会CD,ブランドCD,アイテムCD,シーズンCD,素材CD,デザイナーCD,
               メーカーCD,原産国CD,元上代,上代,売変日,原価,営業原価,加工工賃,デリバリー日,納品日,
               店頭投入日,JANコード1,JANコード2,JANコード3,洗濯表示,絵型名,メモ,消費税計算方法,在庫管理FLG,消費税CD, 
               名称CD01,名称CD02,名称CD03,名称CD04,名称CD05,名称CD06,名称CD07,名称CD08,名称CD09,名称CD10,
               自動配分FLG,販売期限,商品区分FLG,商品サイズ区分,基準倉庫CD,ゼロ単価区分,JAN先頭桁,POS区分,
               予備01,予備02,予備03,予備04,予備05,予備06,予備07,予備08,予備09,予備10,
               予備11,予備12,予備13,予備14,予備15,予備16,予備17,予備18,予備19,予備20,
               仕入区分,消化桁切指定,消化端数区分,消化計算区分,消化掛率,男女区分,コラボ出力区分,代表品番FLG, セール区分,リピート日,
               メーカー品番,仕入価格,納品区分,絵型名2,販売開始日,外貨単価,EC連携,EC取置
        """;

        #region Combobox List 
        // 在庫管理 
        [ObservableProperty]
        public Dictionary<int, string>? m_InvMngmentList;
        // セール区分 
        [ObservableProperty]
        public Dictionary<int, string>? m_salesCateList;
        // autoDistFLG 
        [ObservableProperty]
        public Dictionary<int, string>? m_autoDistList;
        // 納品区分 
        [ObservableProperty]
        public Dictionary<int, string>? m_deliverCateList;
        // EC連携 
        [ObservableProperty]
        public Dictionary<int, string>? m_ecConnectList;
        // EC取置 
        [ObservableProperty]
        public Dictionary<int, string>? m_ecReserveList;
        // 仕入区分
        [ObservableProperty]
        public Dictionary<int, string>? m_purchCateList;
        // 消化計算
        [ObservableProperty]
        public Dictionary<int, string>? m_dgCalcList;
        // 消化桁切
        [ObservableProperty]
        public Dictionary<int, string>? m_dgCutOffList;
        // 消化端数
        [ObservableProperty]
        public Dictionary<int, string>? m_conPurcCateList;
        // POS区分
        [ObservableProperty]
        public Dictionary<int, string>? m_posCateList;
        // 代表品番FLG
        [ObservableProperty]
        public Dictionary<int, string>? m_represNoFlgList;
        // 商品区分FLG
        [ObservableProperty]
        public Dictionary<int, string>? m_prodCateFlgList;
        // 消費税CD (軽減税率)
        [ObservableProperty]
        public Dictionary<int, string>? m_taxCDList;
        // 商品サイズ区分
        [ObservableProperty]
        public Dictionary<string, string>? m_prodSizCateList;
        // ゼロ単価区分
        [ObservableProperty]
        public Dictionary<int, string>? m_zeroPriCateList;
        // 消費税計算方法
        [ObservableProperty]
        public Dictionary<int, string>? m_taxCalcList;
        // コラボ出力区分 (下札サイズ)
        [ObservableProperty]
        public Dictionary<int, string>? m_colOutCateList;
        #endregion

        #region Dialog Search
        [RelayCommand]
        public void SelBrand(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.BrandCD = get_sel00.Code;
                EditProduct.BrandName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelItem(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ItemCD = get_sel00.Code;
                EditProduct.ItemName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelSeason(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.SeasonCD = get_sel00.Code;
                EditProduct.SeasonName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelMaterial(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.MaterialCD = get_sel00.Code;
                EditProduct.MaterialName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelDesign(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.DesignCD = get_sel00.Code;
                EditProduct.DesignName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelMadeIn(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.MadeInCD = get_sel00.Code;
                EditProduct.MadeInName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelExhibit(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ExhibitCD = get_sel00.Code;
                EditProduct.ExhibitName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelManufact(object value)
        {
            var get_sel00 = (SelValueModel)value;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.ManufactCD = get_sel00.Code;
                EditProduct.ManufactName = get_sel00.Name;
            }
        }
        [RelayCommand]
        public void SelName((object Result1, object Result2) value)
        {
            var (result1, result2) = value;

            var get_sel00 = (SelValueModel)result1;
            string kubun = string.Empty;
            if (result2 is string res2) kubun = res2;

            if (get_sel00 != null && EditProduct != null)
            {
                if (kubun == "B01")
                {
                    EditProduct.NameCD01 = get_sel00.Code;
                    EditProduct.SubName01 = get_sel00.Name;
                }
                if (kubun == "B02")
                {
                    EditProduct.NameCD02 = get_sel00.Code;
                    EditProduct.SubName02 = get_sel00.Name;
                }
                if (kubun == "B03")
                {
                    EditProduct.NameCD03 = get_sel00.Code;
                    EditProduct.SubName03 = get_sel00.Name;
                }
                if (kubun == "B04")
                {
                    EditProduct.NameCD04 = get_sel00.Code;
                    EditProduct.SubName04 = get_sel00.Name;
                }
                if (kubun == "B05")
                {
                    EditProduct.NameCD05 = get_sel00.Code;
                    EditProduct.SubName05 = get_sel00.Name;
                }
                if (kubun == "B06")
                {
                    EditProduct.NameCD06 = get_sel00.Code;
                    EditProduct.SubName06 = get_sel00.Name;
                }
                if (kubun == "B07")
                {
                    EditProduct.NameCD07 = get_sel00.Code;
                    EditProduct.SubName07 = get_sel00.Name;
                }
                if (kubun == "B08")
                {
                    EditProduct.NameCD08 = get_sel00.Code;
                    EditProduct.SubName08 = get_sel00.Name;
                }
                if (kubun == "B09")
                {
                    EditProduct.NameCD09 = get_sel00.Code;
                    EditProduct.SubName09 = get_sel00.Name;
                }
                if (kubun == "B10")
                {
                    EditProduct.NameCD10 = get_sel00.Code;
                    EditProduct.SubName10 = get_sel00.Name;
                }
            }
        }
        [RelayCommand]
        public void SelStandWare(object value)
        {
            //var get_sel00 = (SelValueModel)value;
            // Note: Require Call SelTok to get ID before Start GetSell00 
            /*
            var get_sel00 = AppData.DlgService.GetSel00("倉庫");
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.StandWareCD = get_sel00.SelectedValue?.Code;
                EditProduct.StockName = get_sel00?.SelectedValue?.Name;
            }
            */
        }
        [RelayCommand]
        public void SelReserve((object Result1, object Result2) value)
        {
            var (result1, result2) = value;
            var get_sel00 = (SelValueModel)result1;
            string kubun = string.Empty;
            if (result2 is string res2) kubun = res2;

            if (get_sel00 != null && EditProduct != null)
            {
                if (kubun == "Y01")
                {
                    EditProduct.Reserve01 = get_sel00.Code;
                    EditProduct.ReserveName01 = get_sel00.Name;
                }
                if (kubun == "Y02")
                {
                    EditProduct.Reserve02 = get_sel00.Code;
                    EditProduct.ReserveName02 = get_sel00.Name;
                }
                if (kubun == "Y03")
                {
                    EditProduct.Reserve03 = get_sel00.Code;
                    EditProduct.ReserveName03 = get_sel00.Name;
                }
                if (kubun == "Y04")
                {
                    EditProduct.Reserve04 = get_sel00.Code;
                    EditProduct.ReserveName04 = get_sel00.Name;
                }
                if (kubun == "Y05")
                {
                    EditProduct.Reserve05 = get_sel00.Code;
                    EditProduct.ReserveName05 = get_sel00.Name;
                }
                if (kubun == "Y06")
                {
                    EditProduct.Reserve06 = get_sel00.Code;
                    EditProduct.ReserveName06 = get_sel00.Name;
                }
                if (kubun == "Y07")
                {
                    EditProduct.Reserve07 = get_sel00.Code;
                    EditProduct.ReserveName07 = get_sel00.Name;
                }
                if (kubun == "Y08")
                {
                    EditProduct.Reserve08 = get_sel00.Code;
                    EditProduct.ReserveName08 = get_sel00.Name;
                }
                if (kubun == "Y09")
                {
                    EditProduct.Reserve09 = get_sel00.Code;
                    EditProduct.ReserveName09 = get_sel00.Name;
                }
                if (kubun == "Y10")
                {
                    EditProduct.Reserve10 = get_sel00.Code;
                    EditProduct.ReserveName10 = get_sel00.Name;
                }
                if (kubun == "Y11")
                {
                    EditProduct.Reserve11 = get_sel00.Code;
                    EditProduct.ReserveName11 = get_sel00.Name;
                }
                if (kubun == "Y12")
                {
                    EditProduct.Reserve12 = get_sel00.Code;
                    EditProduct.ReserveName12 = get_sel00.Name;
                }
                if (kubun == "Y13")
                {
                    EditProduct.Reserve13 = get_sel00.Code;
                    EditProduct.ReserveName13 = get_sel00.Name;
                }
                if (kubun == "Y14")
                {
                    EditProduct.Reserve14 = get_sel00.Code;
                    EditProduct.ReserveName14 = get_sel00.Name;
                }
                if (kubun == "Y15")
                {
                    EditProduct.Reserve15 = get_sel00.Code;
                    EditProduct.ReserveName15 = get_sel00.Name;
                }
                if (kubun == "Y16")
                {
                    EditProduct.Reserve16 = get_sel00.Code;
                    EditProduct.ReserveName16 = get_sel00.Name;
                }
                if (kubun == "Y17")
                {
                    EditProduct.Reserve17 = get_sel00.Code;
                    EditProduct.ReserveName17 = get_sel00.Name;
                }
                if (kubun == "Y18")
                {
                    EditProduct.Reserve18 = get_sel00.Code;
                    EditProduct.ReserveName18 = get_sel00.Name;
                }
                if (kubun == "Y19")
                {
                    EditProduct.Reserve19 = get_sel00.Code;
                    EditProduct.ReserveName19 = get_sel00.Name;
                }
                if (kubun == "Y20")
                {
                    EditProduct.Reserve20 = get_sel00.Code;
                    EditProduct.ReserveName20 = get_sel00.Name;
                }
            }
        }
        [RelayCommand]
        public void SelGenderCate((object Result1, object Result2) value)
        {
            var (result1, result2) = value;
            var get_sel00 = (SelValueModel)result1;
            if (get_sel00 != null && EditProduct != null)
            {
                EditProduct.GenderCate = get_sel00.Code;
                EditProduct.GenderCateName = get_sel00.Name;
            }
        }
        #endregion

        #region Button Events
        /// <summary>
        /// 表示更新(F5)
        /// </summary>
        [RelayCommand]
        public void DoSearch()
        {
            /* ダイアログ検索条件を追加 */
            if (AppData.ClassCvnet.MstDialog.ContainsKey("商品") && AppData.ClassCvnet.ComboListFLg == 1)
            {
                var ar = new string[] { "1" };
                var vm = AppData.DlgService.GetSelSho(AppData.ClassCvnet.MstDialog["商品"].v_mstname, null, ar);
                if (vm != null)
                {
                    if (sv_para == null) sv_para = new BizArray();
                    else sv_para.Clear();
                    sv_sql = "";

                    sv_para = vm.SelShoResult1.Item2;
                    sv_sql = vm.SelShoResult1.Item1;
                    OnQuery(vm.SelShoResult1.Item2, vm.SelShoResult1.Item1);
                }
                return;
            }
            var v_para = new BizArray();
            v_para[0] = SelProductCd;
            OnQuery(v_para);
        }

        /// <summary>
        /// 表示更新 (<<) Previous Page 
        /// </summary>
        [RelayCommand]
        private void DoPrevSearch()
        {
            if (ListProduct?.Count == 0) return;
            if (AppData.ClassCvnet.MstDialog.ContainsKey("商品") && AppData.ClassCvnet.ComboListFLg == 1)
            {
                if (sv_para?.Count == 0) return;

                /* 元パラメータセーブ */
                var v_para0 = sv_para[0];
                var v_para1 = sv_para[1];

                sv_para[0] = sv_para[0];
                sv_para[1] = ListProduct.FirstOrDefault().ProductCD;
                OnQuery(sv_para, sv_sql, "desc");

                /* 元パラメータロード */
                sv_para[0] = v_para0;
                sv_para[1] = v_para1;
                return;
            }
            var old_pos = new BizArray();
            old_pos[0] = SelProductCd;
            if (ListProduct?.Count > 0)
                SelProductCd = ListProduct[0].ProductCD;
            var v_para = new BizArray();
            v_para[0] = SelProductCd;
            OnQuery(v_para, "", "desc");
            SelProductCd = old_pos[0];
        }

        /// <summary>
        /// 表示更新 (>>) Next Page
        /// </summary>
        [RelayCommand]
        private void DoNextSearch()
        {
            if (ListProduct?.Count == 0) return;
            if (AppData.ClassCvnet.MstDialog.ContainsKey("商品") && AppData.ClassCvnet.ComboListFLg == 1)
            {
                if (sv_para?.Count == 0) return;

                /* 元パラメータセーブ */
                var v_para0 = sv_para[0];
                var v_para1 = sv_para[1];

                sv_para[0] = ListProduct.LastOrDefault().ProductCD;
                sv_para[1] = sv_para[1];
                OnQuery(sv_para, sv_sql);

                /* 元パラメータロード */
                sv_para[0] = v_para0;
                sv_para[1] = v_para1;
                return;
            }
            var old_pos = new BizArray();
            old_pos[0] = SelProductCd;
            if (ListProduct?.Count > 0)
                SelProductCd = ListProduct.LastOrDefault().ProductCD;
            var v_para = new BizArray();
            v_para[0] = SelProductCd;
            OnQuery(v_para, "", "");
            SelProductCd = old_pos[0];
        }

        /// <summary>
        /// 品質表示 (S+F10)
        /// </summary>
        [RelayCommand]
        private void DoQualityDsp()
        {
            if (EditProduct == null) return;
            if (!string.IsNullOrEmpty(EditProduct?.ProductCD))
            {
                if (SelectedProduct == null)
                    ClientLib.MessageBoxError(this, "まず商品マスタを修正or追加実行して下さい。");
                else if (EditProduct?.ProductCD != SelectedProduct.ProductCD)
                    ClientLib.MessageBoxError(this, "まず商品マスタを修正or追加実行して下さい。");
                else
                {
                    var wrk_para = new BizArray();
                    wrk_para[0] = EditProduct?.ProductCD;
                    wrk_para[1] = EditProduct?.ProductName;
                    wrk_para[2] = EditProduct?.SeqNo.ToString();
                    wrk_para[3] = EditProduct?.VdateUpdate.ToString();
                    /* 参照FLG追加 10.10.7 */
                    var sflg = int.TryParse(para[0], out int _flg) ? _flg : 0;

                    var vm_result = AppData.DlgService.GetShoSh2(wrk_para.ToArray(), sflg);
                    if (vm_result != null)
                    {
                        if (vm_result.resp_code == -1) 
                            MessageBox.Show("他で商品マスタが更新中です", "エラー", MessageBoxButton.OK);
                        else if (vm_result.resp_code == -2)
                            MessageBox.Show("他で更新されていますので、登録されていません", "エラー", MessageBoxButton.OK);
                        else if (vm_result.resp_code < 0)
                            MessageBox.Show("商品マスタのロックエラーです", "エラー", MessageBoxButton.OK);
                    }
                }
            }
        }

        /// <summary>
        /// 原価枝番(S+F11)
        /// </summary>
        [RelayCommand]
        private void DoCostBranch()
        {
            if (!string.IsNullOrEmpty(EditProduct?.ProductCD))
            {
                if (SelectedProduct == null)
                    MessageBox.Show("まず商品マスタを修正or追加実行して下さい。", "エラー", MessageBoxButton.OK);
                else if (EditProduct?.ProductCD != SelectedProduct.ProductCD)
                    MessageBox.Show("まず商品マスタを修正or追加実行して下さい。", "エラー", MessageBoxButton.OK);
                else
                {
                    /* 最終仕入 09.12.09 */
                    if (AppData.ClassCvnet.config.LastSiire != 0 || AppData.ClassCvnet.config.souheikin != 0)
                    {
                        var wrk_para = new BizArray();
                        wrk_para[0] = EditProduct?.ProductCD;
                        AppData.DlgService.GetSelghn(wrk_para.ToArray());
                        return;
                    }

                    var wrk2_para = new BizArray();
                    wrk2_para[0] = EditProduct?.ProductCD;
                    wrk2_para[1] = EditProduct?.OriPrice.ToString();
                    wrk2_para[2] = EditProduct?.Price.ToString();
                    wrk2_para[3] = EditProduct?.Cost.ToString();
                    wrk2_para[4] = EditProduct?.OpCostPrice.ToString();
                    wrk2_para[5] = EditProduct?.ManufactFee.ToString();
                    wrk2_para[6] = SelectedProduct.SeqNo.ToString();
                    wrk2_para[7] = SelectedProduct.VdateUpdate.ToString();
                    wrk2_para[8] = EditProduct?.PurchasePrice.ToString();
                    /* 商品CD,元上代,上代,原価,営業原価,加工工賃 */
                    /* [6][7] SEQ_NO,VDATE_UPDATE */
                    /* 参照FLG追加 10.10.7 */
                    int sflg = int.TryParse(para[0], out int _sflg) ? _sflg : 0;
                    var ret_val = AppData.DlgService.GetShoSh3(wrk2_para.ToArray(), sflg);
                    if (ret_val.resp_code == -1)
                        MessageBox.Show("他で商品マスタが更新中です", "エラー", MessageBoxButton.OK);
                    else if (ret_val.resp_code == -2)
                        MessageBox.Show("他で更新されていますので、登録されていません", "エラー", MessageBoxButton.OK);
                    else if (ret_val.resp_code < 0)
                        MessageBox.Show("商品マスタのロックエラーです", "エラー", MessageBoxButton.OK);
                }
            }
        }

        /// <summary>
        /// 色サイズ展開(S+F12)
        /// </summary>
        [RelayCommand]
        private void DoColorSiz()
        {
            if (!string.IsNullOrEmpty(EditProduct?.ProductCD))
            {
                if (SelectedProduct == null)
                    MessageBox.Show("まず商品マスタを修正or追加実行して下さい。", "エラー", MessageBoxButton.OK);
                else if (EditProduct?.ProductCD != SelectedProduct.ProductCD)
                    MessageBox.Show("まず商品マスタを修正or追加実行して下さい。", "エラー", MessageBoxButton.OK);
                else if (EditProduct?.ProdSizeCate != SelectedProduct.ProdSizeCate)
                    MessageBox.Show("サイズ区分が変更されています。\n商品マスタを修正or追加実行して下さい。", "エラー", MessageBoxButton.OK);
                else
                {
                    var wrk_para = new BizArray();
                    wrk_para[0] = EditProduct?.ProductCD;
                    wrk_para[1] = SelectedProduct.SeqNo.ToString();
                    wrk_para[2] = SelectedProduct.VdateUpdate.ToString();
                    wrk_para[3] = EditProduct?.OriPrice.ToString();
                    wrk_para[4] = EditProduct?.Price.ToString();
                    wrk_para[5] = EditProduct?.ProdSizeCate;
                    /* 参照FLG追加 10.10.7 */
                    int sflg = int.TryParse(para[0], out int _sflg) ? _sflg : 0;
                    var ret_val = AppData.DlgService.GetShoSh5v2(wrk_para.ToArray(), sflg);
                    if (ret_val.resp_code == -1)
                        MessageBox.Show("他で商品マスタが更新中です", "エラー", MessageBoxButton.OK);
                    else if (ret_val.resp_code == -2)
                        MessageBox.Show("他で更新されていますので、登録されていません", "エラー", MessageBoxButton.OK);
                    else if (ret_val.resp_code < 0)
                        MessageBox.Show("商品マスタのロックエラーです", "エラー", MessageBoxButton.OK);
                }
            }
        }
        #endregion

        #region Events
        partial void OnSelectedProductChanged(MasterShohin? value)
        {
            if (value != null)
                EditProduct = Common.CloneObject(value);
            else
                EditProduct = null;
        }
        partial void OnEditProductChanged(MasterShohin? value)
        {
            if (value != null)
            {
                value.PropertyChanged += EditProduct_PropertyChanged; 
            }
        }
        private void EditProduct_PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(MasterShohin.BrandName))
            {
                // Combobox changed
            }
        }
        #endregion

        #region Function
        /// <summary>
        /// 最新の展示会CDをセット 2008.09.25 追加
        /// </summary>
        private void GetTenjikai()
        {
            if (AppData.ClassCvnet.config.TenjiShoki == 1)
            { 
                string sql_str = "SELECT 名称CD, 名称"
                                + " FROM HC$master_meisho"
                                + " WHERE 名称区分='TNJ' AND rownum<=1"
                                + " ORDER BY 名称CD desc";
                var wrk_csv = AppData.Http?.AspxSqlQuery(sql_str);
                if (wrk_csv != null && wrk_csv.Rows.Count == 1)
                {
                    if (EditProduct != null)
                    {
                        EditProduct.ExhibitCD = wrk_csv.Rows[0][0].ToString();
                        EditProduct.ExhibitName = wrk_csv.Rows[0][1].ToString();
                    }
                    if (Shohin3Opt != null)
                    {
                        Shohin3Opt.TxtExhibitReadOnly = true;
                        Shohin3Opt.BtExhibitAct = false;
                    }
                }
            }
        }

        private void SetMode(string[] init_para = null)
        {
            /* JAN→各名称：False、名称→JAN：スルー */
            if (init_para != null) { }
        }

        /// <summary>
        /// 画面表示更新用検索処理 
        /// </summary>
        private void OnQuery(BizArray v_para1, string qs = "", string p_sort = "")
        {
            /* 戻り対応 */
            var v_sort = " asc ";
            if (!string.IsNullOrEmpty(p_sort)) v_sort = " desc";
            var v_hugo = ">=";
            if (!string.IsNullOrEmpty(p_sort)) v_hugo = "<=";

            DataTable ret_csv = new DataTable();

            var v_para = new BizArray();
            for (int i = 0; i < v_para1.Count; i++)
            {
                v_para[i] = v_para1[i];
            }

            var sql_query = "select A.SEQ_NO,A.VDATE_CREATE,A.VDATE_UPDATE";
            for (int i = 0; i < col_list.Count; i++)
                sql_query += ",A." + col_list[i];

            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='TNJ' and H.名称CD=A.展示会CD),'.') 展示会名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='BRD' and H.名称CD=A.ブランドCD),'.') ブランド名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='ITM' and H.名称CD=A.アイテムCD),'.') アイテム名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='SZN' and H.名称CD=A.シーズンCD),'.') シーズン名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='SZI' and H.名称CD=A.素材CD),'.') 素材名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='DZN' and H.名称CD=A.デザイナーCD),'.') デザイナー名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='MKR' and H.名称CD=A.メーカーCD),'.') メーカー名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='GEN' and H.名称CD=A.原産国CD),'.') 原産国名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B01' and H.名称CD=A.名称CD01),'.') 補足01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B02' and H.名称CD=A.名称CD02),'.') 補足02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B03' and H.名称CD=A.名称CD03),'.') 補足03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B04' and H.名称CD=A.名称CD04),'.') 補足04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B05' and H.名称CD=A.名称CD05),'.') 補足05名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B06' and H.名称CD=A.名称CD06),'.') 補足06名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B07' and H.名称CD=A.名称CD07),'.') 補足07名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B08' and H.名称CD=A.名称CD08),'.') 補足08名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B09' and H.名称CD=A.名称CD09),'.') 補足09名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='B10' and H.名称CD=A.名称CD10),'.') 補足10名";
            sql_query += ",NVL((select H.得意先名 from HC$MASTER_TOKUI H where H.得意先CD=A.基準倉庫CD),'.') 倉庫名";
            /* 最終修正者追加対応 20070111 */
            sql_query += ",A.入力社員CD ||' '||(select S.名前 from HC$MASTER_SHAIN S where S.社員CD=A.入力社員CD) 最終修正者";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='MLK' and H.名称CD=A.男女区分),'.') 男女区分名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='STH' and H.名称CD=A.洗濯表示),'.') 洗濯表示名";
            sql_query += ",DECODE(A.承認FLG,1,'済','未済') 承認";
            sql_query += ",DECODE((select count(*) d from hc$master_shohin_genka_henkou sgh where A.商品CD = sgh.商品CD), 0, 0, 1) 商品原価変更履歴有無FLG";

            /* 2010.12.16 外貨単価対応 S */
            /* sql_query += ",A.外貨単価"; */ /* 107列目 */
            /* 2010.12.16 外貨単価対応 E */

            /* 2017.11.16 ide 予備名 追加対応 S */
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y01' and H.名称CD=A.予備01),'.') 予備01名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y02' and H.名称CD=A.予備02),'.') 予備02名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y03' and H.名称CD=A.予備03),'.') 予備03名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y04' and H.名称CD=A.予備04),'.') 予備04名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y05' and H.名称CD=A.予備05),'.') 予備05名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y06' and H.名称CD=A.予備06),'.') 予備06名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y07' and H.名称CD=A.予備07),'.') 予備07名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y08' and H.名称CD=A.予備08),'.') 予備08名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y09' and H.名称CD=A.予備09),'.') 予備09名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y10' and H.名称CD=A.予備10),'.') 予備10名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y11' and H.名称CD=A.予備11),'.') 予備11名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y12' and H.名称CD=A.予備12),'.') 予備12名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y13' and H.名称CD=A.予備13),'.') 予備13名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y14' and H.名称CD=A.予備14),'.') 予備14名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y15' and H.名称CD=A.予備15),'.') 予備15名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y16' and H.名称CD=A.予備16),'.') 予備16名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y17' and H.名称CD=A.予備17),'.') 予備17名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y18' and H.名称CD=A.予備18),'.') 予備18名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y19' and H.名称CD=A.予備19),'.') 予備19名";
            sql_query += ",NVL((select H.名称 from HC$MASTER_MEISHO H where H.名称区分='Y20' and H.名称CD=A.予備20),'.') 予備20名";

            /* 2017.10.05 ide 予備名 追加対応 E */
            sql_query += " from HC$Master_SHOHIN A";

            if (string.IsNullOrEmpty(qs))
            {
                sql_query += " where A.商品CD " + v_hugo + " :1";
            }
            else sql_query += " where A.商品CD in (" + qs + ")";

            sql_query += " order by A.商品CD " + v_sort;
            sql_query = AppData.ClassCvnet.GetSqlDisp(sql_query);

            ret_csv = AppData.Http?.AspxSqlQuery(sql_query, v_para.ToArray());
            if (ret_csv == null || ret_csv.Rows.Count == 0) return;
            try
            {
                var list = (from DataRow dr in ret_csv.Rows
                            select new MasterShohin
                            {
                                SeqNo = Convert.ToInt64(dr["SEQ_NO"]),
                                VdateCreate =  Convert.ToDecimal(dr["VDATE_CREATE"]),
                                VdateUpdate = Convert.ToDecimal(dr["VDATE_UPDATE"]),
                                ProductCD = dr["商品CD"].ToString() ?? string.Empty,
                                ProductName = dr["商品名"].ToString() ?? string.Empty,
                                Abbreviation = dr["略称"].ToString() ?? string.Empty,
                                OldCode = dr["旧コード"].ToString() ?? string.Empty,
                                ExhibitCD = dr["展示会CD"].ToString() ?? string.Empty,
                                BrandCD = dr["ブランドCD"].ToString() ?? string.Empty,
                                ItemCD = dr["アイテムCD"].ToString() ?? string.Empty,
                                SeasonCD = dr["シーズンCD"].ToString() ?? string.Empty,
                                MaterialCD = dr["素材CD"].ToString() ?? string.Empty,
                                DesignCD = dr["デザイナーCD"].ToString() ?? string.Empty,
                                ManufactCD = dr["メーカーCD"].ToString() ?? string.Empty,
                                MadeInCD = dr["原産国CD"].ToString() ?? string.Empty,
                                OriPrice = long.TryParse(dr["元上代"].ToString(), out var _oriprice) ? _oriprice : 0,
                                Price = long.TryParse(dr["上代"].ToString(), out var _price) ? _price : 0,
                                PriceChgDate = DateTime.TryParseExact(dr["売変日"].ToString(), "yyyyMMdd",
                                               CultureInfo.InvariantCulture, DateTimeStyles.None,
                                               out var _priChgDate) ? _priChgDate : new DateTime(1901, 1, 1),
                                Cost = long.TryParse(dr["原価"].ToString(), out var _cost) ? _cost : 0,
                                OpCostPrice = long.TryParse(dr["営業原価"].ToString(), out var _opcostprice) ? _opcostprice : 0,
                                ManufactFee = long.TryParse(dr["加工工賃"].ToString(), out var _manufactfee) ? _manufactfee : 0,
                                CustDeliDate = DateTime.TryParseExact(dr["デリバリー日"].ToString(), "yyyyMMdd",
                                               CultureInfo.InvariantCulture, DateTimeStyles.None,
                                               out var _custDeliDate) ? _custDeliDate : new DateTime(1901, 1, 1),
                                DeliveryDate = DateTime.TryParseExact(dr["納品日"].ToString(), "yyyyMMdd",
                                               CultureInfo.InvariantCulture, DateTimeStyles.None,
                                               out var _deliverDate) ? _deliverDate : new DateTime(1901, 1, 1),
                                InitLaunchDate = DateTime.TryParseExact(dr["店頭投入日"].ToString(), "yyyyMMdd",
                                                 CultureInfo.InvariantCulture, DateTimeStyles.None,
                                                 out var _launchDate) ? _launchDate : new DateTime(1901, 1, 1),
                                JanCode1 = dr["JANコード1"].ToString() ?? string.Empty,
                                JanCode2 = dr["JANコード2"].ToString() ?? string.Empty,
                                JanCode3 = dr["JANコード3"].ToString() ?? string.Empty,
                                CareLabel = dr["洗濯表示"].ToString() ?? string.Empty,
                                ImgName = dr["絵型名"].ToString() ?? string.Empty,
                                Memo = dr["メモ"].ToString() ?? string.Empty,
                                TaxCalcMethod = int.TryParse(dr["消費税計算方法"].ToString(), out var _tax_calc) ? _tax_calc : 0,
                                InvMngmentFLG = Convert.ToInt32(dr["在庫管理FLG"]),
                                TaxCD = long.TryParse(dr["消費税CD"].ToString(), out var _taxcd) ? _taxcd : 0,
                                NameCD01 = dr["名称CD01"].ToString() ?? string.Empty,
                                NameCD02 = dr["名称CD02"].ToString() ?? string.Empty,
                                NameCD03 = dr["名称CD03"].ToString() ?? string.Empty,
                                NameCD04 = dr["名称CD04"].ToString() ?? string.Empty,
                                NameCD05 = dr["名称CD05"].ToString() ?? string.Empty,
                                NameCD06 = dr["名称CD06"].ToString() ?? string.Empty,
                                NameCD07 = dr["名称CD07"].ToString() ?? string.Empty,
                                NameCD08 = dr["名称CD08"].ToString() ?? string.Empty,
                                NameCD09 = dr["名称CD09"].ToString() ?? string.Empty,
                                NameCD10 = dr["名称CD10"].ToString() ?? string.Empty,
                                AutoDistFLG = Convert.ToInt32(dr["自動配分FLG"]),
                                SalesPeriod = dr["販売期限"].ToString() ?? string.Empty,
                                ProdCateFLG = Convert.ToInt32(dr["商品区分FLG"]),
                                ProdSizeCate = dr["商品サイズ区分"].ToString() ?? string.Empty,
                                StandWareCD = dr["基準倉庫CD"].ToString() ?? string.Empty,
                                ZeroPriceCate = Convert.ToInt32(dr["ゼロ単価区分"]),
                                Jan1stdigit = dr["JAN先頭桁"].ToString() ?? string.Empty,
                                PosCate = Convert.ToInt32(dr["POS区分"]),
                                Reserve01 = dr["予備01"].ToString() ?? string.Empty,
                                Reserve02 = dr["予備02"].ToString() ?? string.Empty,
                                Reserve03 = dr["予備03"].ToString() ?? string.Empty,
                                Reserve04 = dr["予備04"].ToString() ?? string.Empty,
                                Reserve05 = dr["予備05"].ToString() ?? string.Empty,
                                Reserve06 = dr["予備06"].ToString() ?? string.Empty,
                                Reserve07 = dr["予備07"].ToString() ?? string.Empty,
                                Reserve08 = dr["予備08"].ToString() ?? string.Empty,
                                Reserve09 = dr["予備09"].ToString() ?? string.Empty,
                                Reserve10 = dr["予備10"].ToString() ?? string.Empty,
                                Reserve11 = dr["予備11"].ToString() ?? string.Empty,
                                Reserve12 = dr["予備12"].ToString() ?? string.Empty,
                                Reserve13 = dr["予備13"].ToString() ?? string.Empty,
                                Reserve14 = dr["予備14"].ToString() ?? string.Empty,
                                Reserve15 = dr["予備15"].ToString() ?? string.Empty,
                                Reserve16 = dr["予備16"].ToString() ?? string.Empty,
                                Reserve17 = dr["予備17"].ToString() ?? string.Empty,
                                Reserve18 = dr["予備18"].ToString() ?? string.Empty,
                                Reserve19 = dr["予備19"].ToString() ?? string.Empty,
                                Reserve20 = dr["予備20"].ToString() ?? string.Empty,
                                PurchaseCate = Convert.ToInt32(dr["仕入区分"]),
                                DgCutOffSpec = Convert.ToInt32(dr["消化桁切指定"]),
                                ConsignPurcCate = Convert.ToInt32(dr["消化端数区分"]),
                                DgCalcCate = Convert.ToInt32(dr["消化計算区分"]),
                                ConsignPurcRate = Convert.ToInt32(dr["消化掛率"]),
                                GenderCate = dr["男女区分"].ToString() ?? string.Empty,
                                CollabOutCate = Convert.ToInt32(dr["コラボ出力区分"]),
                                RepresentNoFLG = Convert.ToInt32(dr["代表品番FLG"]),
                                SalesCate = Convert.ToInt32(dr["セール区分"]),
                                RepeatDate = DateTime.TryParseExact(dr["リピート日"].ToString(), "yyyyMMdd",
                                             CultureInfo.InvariantCulture, DateTimeStyles.None,
                                             out var _repeatDate) ? _repeatDate : new DateTime(1901, 1, 1),
                                MakerNo = dr["メーカー品番"].ToString() ?? string.Empty,
                                PurchasePrice = long.TryParse(dr["仕入価格"].ToString(), out var _purh_price) ? _purh_price : 0,
                                DeliveryCate = Convert.ToInt32(dr["納品区分"]),
                                ImgName2 = dr["絵型名2"].ToString() ?? string.Empty,
                                SaleStDate = DateTime.TryParseExact(dr["販売開始日"].ToString(), "yyyyMMdd",
                                             CultureInfo.InvariantCulture, DateTimeStyles.None,
                                             out var _saleStDate) ? _saleStDate : new DateTime(1901, 1, 1),
                                ForeignCurPrice = long.TryParse(dr["外貨単価"].ToString(), out var _foreignprice) ? _foreignprice : 0,
                                EcConnect = Convert.ToInt32(dr["EC連携"]),
                                EcReserve = Convert.ToInt32(dr["EC取置"]),
                                ExhibitName = dr["展示会名"].ToString() ?? string.Empty,
                                BrandName = dr["ブランド名"].ToString() ?? string.Empty,
                                ItemName = dr["アイテム名"].ToString() ?? string.Empty,
                                SeasonName = dr["シーズン名"].ToString() ?? string.Empty,
                                MaterialName = dr["素材名"].ToString() ?? string.Empty,
                                DesignName = dr["デザイナー名"].ToString() ?? string.Empty,
                                ManufactName = dr["メーカー名"].ToString() ?? string.Empty,
                                MadeInName = dr["原産国名"].ToString() ?? string.Empty,
                                SubName01 = dr["補足01名"].ToString() ?? string.Empty,
                                SubName02 = dr["補足02名"].ToString() ?? string.Empty,
                                SubName03 = dr["補足03名"].ToString() ?? string.Empty,
                                SubName04 = dr["補足04名"].ToString() ?? string.Empty,
                                SubName05 = dr["補足05名"].ToString() ?? string.Empty,
                                SubName06 = dr["補足06名"].ToString() ?? string.Empty,
                                SubName07 = dr["補足07名"].ToString() ?? string.Empty,
                                SubName08 = dr["補足08名"].ToString() ?? string.Empty,
                                SubName09 = dr["補足09名"].ToString() ?? string.Empty,
                                SubName10 = dr["補足10名"].ToString() ?? string.Empty,
                                StockName = dr["倉庫名"].ToString() ?? string.Empty,
                                ModifiedBy = dr["最終修正者"].ToString() ?? string.Empty,
                                GenderCateName = dr["男女区分名"].ToString() ?? string.Empty,
                                CareLabelName = dr["洗濯表示名"].ToString() ?? string.Empty,
                                Approval = dr["承認"].ToString() ?? string.Empty,
                                CostChgHistFLG = Convert.ToInt32(dr["商品原価変更履歴有無FLG"]),
                                ReserveName01 = dr["予備01名"].ToString() ?? string.Empty,
                                ReserveName02 = dr["予備02名"].ToString() ?? string.Empty,
                                ReserveName03 = dr["予備03名"].ToString() ?? string.Empty,
                                ReserveName04 = dr["予備04名"].ToString() ?? string.Empty,
                                ReserveName05 = dr["予備05名"].ToString() ?? string.Empty,
                                ReserveName06 = dr["予備06名"].ToString() ?? string.Empty,
                                ReserveName07 = dr["予備07名"].ToString() ?? string.Empty,
                                ReserveName08 = dr["予備08名"].ToString() ?? string.Empty,
                                ReserveName09 = dr["予備09名"].ToString() ?? string.Empty,
                                ReserveName10 = dr["予備10名"].ToString() ?? string.Empty,
                                ReserveName11 = dr["予備11名"].ToString() ?? string.Empty,
                                ReserveName12 = dr["予備12名"].ToString() ?? string.Empty,
                                ReserveName13 = dr["予備13名"].ToString() ?? string.Empty,
                                ReserveName14 = dr["予備14名"].ToString() ?? string.Empty,
                                ReserveName15 = dr["予備15名"].ToString() ?? string.Empty,
                                ReserveName16 = dr["予備16名"].ToString() ?? string.Empty,
                                ReserveName17 = dr["予備17名"].ToString() ?? string.Empty,
                                ReserveName18 = dr["予備18名"].ToString() ?? string.Empty,
                                ReserveName19 = dr["予備19名"].ToString() ?? string.Empty,
                                ReserveName20 = dr["予備20名"].ToString() ?? string.Empty,
                            }).OrderBy(c => c.ProductCD).ToList();
                ListProduct = new ObservableCollection<MasterShohin>(list);
                if (ListProduct.Count > 0) SelectedProduct = ListProduct[0];
            }
            catch (Exception ex) { Console.WriteLine(ex.Message); }
        }
        #endregion
    }
    public class JanStyleRec
    {
        public int equalFlg { get; set; }
        public int janlength1 { get; set; }
        public int janlength2 { get; set; }
        public string pttrnStyle { get; set; }
        public DataTable janPattern { get; set; }
        public DataTable Regaxchk { get; set; }
        public DataTable barPattern { get; set; }
        /* 0：JAN先頭桁→各名称、1：各名称→JAN先頭桁（equalFlg立ってれば商品CDも） */
        public int makejan { get; set; }
        public int renbanflg { get; set; }
        /* 070215追加 */
        public int danflg { get; set; }

        public JanStyleRec()
        {
            equalFlg = 0;
            janlength1 = 0;
            janlength2 = 0;
            pttrnStyle = string.Empty;
            janPattern = new DataTable();
            Regaxchk = new DataTable();
            barPattern = new DataTable();
            makejan = 0;
            renbanflg = 0;
            danflg = 0;
        }
    }

    public partial class Shohin3Opt : ObservableObject
    {
        /// <summary>
        /// 商品CD (Text3) MaxLength
        /// </summary>
        [ObservableProperty]
        int m_ProdMaxLen;

        /// <summary>
        /// 商品CD (Text3) IsReadOnly
        /// </summary>
        [ObservableProperty]
        bool m_ProdReadOnly;

        /// <summary>
        /// JAN連番 (Text26) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_JanCode3Vis;

        /// <summary>
        /// JAN連番 (Text49) MaxLength
        /// </summary>
        [ObservableProperty]
        int m_Jan1stMaxLen;

        /// <summary>
        /// JAN連番 (Text49) IsReadOnly
        /// </summary>
        [ObservableProperty]
        bool m_Jan1stReadOnly;

        /// <summary>
        /// 表示更新 (BtReDisp) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtReDispAct;

        /// <summary>
        /// 表示更新 NextBtn (BtReDispNext) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtReDispNextAct;

        /// <summary>
        /// 表示更新 PrevBtn (BtReDispPrev) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtReDispPrevAct;

        /// <summary>
        /// 修正 (Bt_Update) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtUpdateAct;

        /// <summary>
        /// 修正 (Bt_Update) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_BtUpdateVis;

        /// <summary>
        /// 追加 (Bt_Insert) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtInsertAct;

        /// <summary>
        /// 追加 (Bt_Insert) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_BtInsertVis;

        /// <summary>
        /// 削除 (Bt_Update) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtDeleteAct;

        /// <summary>
        /// 削除 (Bt_Update) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_BtDeleteVis;

        /// <summary>
        /// 展示会 (Text7) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_TxtExhibitReadOnly;

        /// <summary>
        /// 展示会 (Button77) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtExhibitAct;

        /// <summary>
        /// Item (Text4 ~ Text89, Except Text49) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_ItemListAct;

        /// <summary>
        /// 補足1
        /// </summary>
        [ObservableProperty]
        string m_LabelA01;

        /// <summary>
        /// 補足2
        /// </summary>
        [ObservableProperty]
        string m_LabelA02;

        /// <summary>
        /// 補足3
        /// </summary>
        [ObservableProperty]
        string m_LabelA03;

        /// <summary>
        /// 補足4
        /// </summary>
        [ObservableProperty]
        string m_LabelA04;

        /// <summary>
        /// 補足5
        /// </summary>
        [ObservableProperty]
        string m_LabelA05;

        /// <summary>
        /// 補足6
        /// </summary>
        [ObservableProperty]
        string m_LabelA06;

        /// <summary>
        /// 補足7
        /// </summary>
        [ObservableProperty]
        string m_LabelA07;

        /// <summary>
        /// 補足8
        /// </summary>
        [ObservableProperty]
        string m_LabelA08;

        /// <summary>
        /// 補足9
        /// </summary>
        [ObservableProperty]
        string m_LabelA09;

        /// <summary>
        /// 補足10
        /// </summary>
        [ObservableProperty]
        string m_LabelA10;

        /// <summary>
        /// 予備01 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB01;

        /// <summary>
        /// 予備02 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB02;

        /// <summary>
        /// 予備03 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB03;

        /// <summary>
        /// 予備04 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB04;

        /// <summary>
        /// 予備05 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB05;

        /// <summary>
        /// 予備06 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB06;

        /// <summary>
        /// 予備07 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB07;

        /// <summary>
        /// 予備08 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB08;

        /// <summary>
        /// 予備09 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB09;

        /// <summary>
        /// 予備10 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB10;

        /// <summary>
        /// 予備11 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB11;

        /// <summary>
        /// 予備12 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB12;

        /// <summary>
        /// 予備13 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB13;

        /// <summary>
        /// 予備14 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB14;

        /// <summary>
        /// 予備15 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB15;

        /// <summary>
        /// 予備16 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB16;

        /// <summary>
        /// 予備17 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB17;

        /// <summary>
        /// 予備18 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB18;

        /// <summary>
        /// 予備19 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB19;

        /// <summary>
        /// 予備20 Title
        /// </summary>
        [ObservableProperty]
        string m_LabelB20;

        /// <summary>
        /// 連携表示 (Bt_Renkei) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_BtRenkeiVis;

        /// <summary>
        /// 連携表示 (Bt_Renkei) IsEnable
        /// </summary>
        [ObservableProperty]
        bool m_BtRenkeiAct;

        /// <summary>
        /// 仕入価格 (Label27) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_LblPurchPriceVis;

        /// <summary>
        /// 仕入価格 (Text82) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_TxtPurchPriceVis;

        /// <summary>
        /// 原価枝番 (Bt_Genka) Title
        /// </summary>
        [ObservableProperty]
        string m_BtGenkaTitle;

        /// <summary>
        /// 外貨仕入価格 (Label86) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_LblForCurPrVis;

        /// <summary>
        /// 外貨仕入価格 (Text86) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_TxtForCurPrVis;

        /// <summary>
        /// 商品CD Label (DspMeicode) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_DspMeicodeVis;

        /// <summary>
        /// 商品CD Text (ImpMeiCode) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_ImpMeiCodeVis;

        /// <summary>
        /// EC連携 (Label38) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_LblEcConnectVis;

        /// <summary>
        /// EC連携 (Text87) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_TxtEcConnectVis;

        /// <summary>
        /// EC取置 (Label39) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_LblEcReserveVis;

        /// <summary>
        /// EC取置 (Text88) Visible
        /// </summary>
        [ObservableProperty]
        Visibility m_TxtEcReserveVis;

        public Shohin3Opt()
        {
            ProdMaxLen = 0;
            ProdReadOnly = false;
            JanCode3Vis = Visibility.Visible;
            Jan1stMaxLen = 0;
            Jan1stReadOnly = false;
            BtReDispAct = true;
            BtReDispNextAct = true;
            BtReDispPrevAct = true;
            BtUpdateAct = true;
            BtUpdateVis = Visibility.Visible;
            BtInsertAct = true;
            BtInsertVis = Visibility.Visible;
            BtDeleteAct = true;
            BtDeleteVis = Visibility.Visible;
            TxtExhibitReadOnly = false;
            BtExhibitAct = true;
            ItemListAct = true;
            LabelA01 = "補足1";
            LabelA02 = "補足2";
            LabelA03 = "補足3";
            LabelA04 = "補足4";
            LabelA05 = "補足5";
            LabelA06 = "補足6";
            LabelA07 = "補足7";
            LabelA08 = "補足8";
            LabelA09 = "補足9";
            LabelA10 = "補足10";
            LabelB01 = "予備01";
            LabelB02 = "予備02";
            LabelB03 = "予備03";
            LabelB04 = "予備04";
            LabelB05 = "予備05";
            LabelB06 = "予備06";
            LabelB07 = "予備07";
            LabelB08 = "予備08";
            LabelB09 = "予備09";
            LabelB10 = "予備10";
            LabelB11 = "予備11";
            LabelB12 = "予備12";
            LabelB13 = "予備13";
            LabelB14 = "予備14";
            LabelB15 = "予備15";
            LabelB16 = "予備16";
            LabelB17 = "予備17";
            LabelB18 = "予備18";
            LabelB19 = "予備19";
            LabelB20 = "予備20";
            BtRenkeiAct = false;
            BtRenkeiVis = Visibility.Collapsed;
            LblPurchPriceVis = Visibility.Visible;
            TxtPurchPriceVis = Visibility.Visible;
            BtGenkaTitle = "原価枝番(S+F11)";
            LblForCurPrVis = Visibility.Visible;
            TxtForCurPrVis = Visibility.Visible;
            DspMeicodeVis = Visibility.Visible;
            ImpMeiCodeVis = Visibility.Visible;
            LblEcConnectVis = Visibility.Visible;
            TxtEcConnectVis = Visibility.Visible;
            LblEcReserveVis = Visibility.Visible;
            TxtEcReserveVis = Visibility.Visible;
        }
    }
}
